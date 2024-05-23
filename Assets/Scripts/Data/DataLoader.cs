using System;
using System.Collections;
using Microsoft.Research.Science.Data;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class DataLoader : MonoBehaviour
{
    static DataLoader m_instance;

    // Used to query the current timeline value of the particles.
    private AbstractMapInterface m_MapInterface;

    private int m_nextRequestIndex = -1;
    private bool m_isLoading;

    public event Action m_onLoadStartEvent;
    public event Action<bool> m_onLoadEndEvent;
    // This event is fired when particles are ready. The arguments to the action are the longitude, latitude and height of the center point of the particles.
    public event Action<float3> m_onParticlesReadyEvent;

    public bool IsLoading => m_isLoading;

    public static DataLoader Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<DataLoader>();
            }
            if (m_instance == null)
            {
                GameObject go = new GameObject("DataLoader");
                DontDestroyOnLoad(go);
                m_instance = go.AddComponent<DataLoader>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Data loader already exists, removing instance");
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // There should be a component in the scene for controlling the particle simulation time.
        m_MapInterface = FindObjectOfType<AbstractMapInterface>();
    }

    void OnLoadStart()
    {
        m_isLoading = true;
        m_onLoadStartEvent?.Invoke();
    }

    void OnLoadEnd(bool a_success)
	{
        m_isLoading = false;
        m_onLoadEndEvent?.Invoke(a_success);
    }

    public void LoadJsonFile(string a_filePath, Action<bool> a_completedCallback)
    {
        string path = "file:///" + a_filePath;
        Debug.Log("Loading JSON from path: " + path);
        OnLoadStart();
        m_isLoading = true;
        StartCoroutine(LoadJSON(UnityWebRequest.Get(path), a_completedCallback, ++m_nextRequestIndex));
    }

    public void LoadJsonURL(string a_url, Action<bool> a_completedCallback)
    {
        Debug.Log("Loading JSON from URL: " + a_url);
        OnLoadStart();
        m_isLoading = true;
        StartCoroutine(LoadJSON(UnityWebRequest.Get(a_url), a_completedCallback, ++m_nextRequestIndex));
    }

    IEnumerator LoadJSON(UnityWebRequest a_request, Action<bool> a_completedCallback, int a_requestIndex)
    {
        yield return a_request.SendWebRequest();

		FlushParticles();
		yield return null; //Wait a frame for the flush

		if (a_request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fetching JSON file failed, message: " + a_request.error);
            a_completedCallback?.Invoke(false);
            OnLoadEnd(false);
        }
        else if (a_requestIndex != m_nextRequestIndex)
        {
            Debug.LogWarning($"Result of old request with ID {a_requestIndex} received. Results are ignored.");
        }
        else
        {
            MemoryTraceWriter traceWriter = new MemoryTraceWriter();
            traceWriter.LevelFilter = System.Diagnostics.TraceLevel.Warning;

            GeoVizData data = JsonConvert.DeserializeObject<GeoVizData>(a_request.downloadHandler.text, new JsonSerializerSettings
            {
                TraceWriter = traceWriter,
                Error = (sender, errorArgs) =>
                {
                    Debug.LogError("Deserialization error: " + errorArgs.ErrorContext.Error);
                }
            });

            if (data != null)
            {
                SetParticleSpawnData(data);
                yield return null; //Wait a frame for the spawn
                a_completedCallback?.Invoke(true);
                OnLoadEnd(true);
            }
            else
            {
                a_completedCallback?.Invoke(false);
                OnLoadEnd(false);
            }
        }

    }

	public void LoadNCDFFile(string a_filePath, Action<bool> a_completedCallback)
	{
		Debug.Log("Loading NCDF from URL: " + a_filePath);
        OnLoadStart();
        m_isLoading = true;
        StartCoroutine(LoadNCDF(a_filePath, a_completedCallback, ++m_nextRequestIndex));
	}

	IEnumerator LoadNCDF(string a_filePath, Action<bool> a_completedCallback, int a_requestIndex)
    {
        bool success = false;
		FlushParticles();

		yield return null; //Wait a frame for the flush
		if (a_requestIndex != m_nextRequestIndex)
		{
			Debug.LogWarning($"Result of old request with ID {a_requestIndex} received. Results are ignored.");
            yield break;
		}

		try
        {
            DataSet ds = DataSet.Open(a_filePath);
			SetParticleSpawnData(ds);
            success = true;
        }
        catch (Exception ex)
        {
            Debug.Log($"Error reading NetCDF file: {ex.Message}");
        }
        finally
		{
            a_completedCallback?.Invoke(success);
            OnLoadEnd(success);
        }
    }

    public void LoadNCDFURL(string a_url, Action<bool> a_completedCallback)
    {
        m_nextRequestIndex++;
        OnLoadStart();

        //TODO
        a_completedCallback?.Invoke(false);
        OnLoadEnd(false);
    }

    void SetParticleSpawnData(GeoVizData a_data)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));

        if (!query.HasSingleton<ParticleVisualizationSettingsData>())
            return;
        Entity root = query.GetSingletonEntity();

        BlobBuilder builder = new BlobBuilder(Allocator.Temp);
        ref ParticlePropertiesBlob ppBlob = ref builder.ConstructRoot<ParticlePropertiesBlob>();
        BlobBuilderArray<float> depthArrayBuilder = builder.Allocate(ref ppBlob.m_depths, a_data.zs.Length);
        BlobBuilderArray<float> latArrayBuilder = builder.Allocate(ref ppBlob.m_lats, a_data.lats.Length);
        BlobBuilderArray<float> lonArrayBuilder = builder.Allocate(ref ppBlob.m_lons, a_data.lons.Length);

        for (int i = 0; i < a_data.lons.Length; i++)
        {
            depthArrayBuilder[i] = a_data.zs[i];
            latArrayBuilder[i] = a_data.lats[i];
            lonArrayBuilder[i] = a_data.lons[i];
        }

        BlobAssetReference<ParticlePropertiesBlob> blobAsset = builder.CreateBlobAssetReference<ParticlePropertiesBlob>(Allocator.Persistent);
		if (entityManager.HasComponent<ParticleSpawnData>(root))
		{
			entityManager.SetComponentData(root, new ParticleSpawnData
			{
				Value = blobAsset,
				m_entriesPerParticle = a_data.entries_per_particle
			});
			entityManager.SetComponentEnabled<ParticleSpawnData>(root, true);
		}
		else
		{
			entityManager.AddComponentData(root, new ParticleSpawnData
			{
				Value = blobAsset,
				m_entriesPerParticle = a_data.entries_per_particle
			});
		}
		builder.Dispose();

		if (entityManager.HasComponent<ParticleTimingData>(root))
		{
			entityManager.SetComponentData(root, new ParticleTimingData
			{
				m_numberIndices = a_data.entries_per_particle,
				m_timePerIndex = 0.5f //TODO: set this
			});
		}
		else
		{
			entityManager.AddComponentData(root, new ParticleTimingData
			{
				m_numberIndices = a_data.entries_per_particle,
				m_timePerIndex = 0.5f //TODO: set this
			});
		}

        StartCoroutine(WaitForParticles());
    }

    void SetParticleSpawnData(DataSet a_data)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
        if (!query.HasSingleton<ParticleVisualizationSettingsData>())
		{
			Debug.LogError("No 'ParticleVisualizationSettingsData' component found. Setting particle spawn data failed.");
			return;
		}
		Entity root = query.GetSingletonEntity();

		// Get the variables 'lon', 'lat', 'z', 'particle_size_distribution', 'particle_classification'
		Array lon = a_data["lon"].GetData();
        Array lat = a_data["lat"].GetData();
        Array z = a_data["z"].GetData();
        //Array particleSizeDistribution = ds["particle_size_distribution"].GetData();
        //Array particleClassification = ds["particle_classification"].GetData();

        int numberParticles = lat.GetLength(0);
        int entriesPerParticle = lat.GetLength(1);
        if (lon.GetLength(1) != entriesPerParticle || entriesPerParticle != z.GetLength(1))
        {
            Debug.LogError("Loaded data has inconsistent length, loading has been aborted.");
			return;
		}


        int dataAmount = lon.GetLength(0) * lon.GetLength(1);
        BlobBuilder builder = new BlobBuilder(Allocator.Temp);
        ref ParticlePropertiesBlob ppBlob = ref builder.ConstructRoot<ParticlePropertiesBlob>();
        BlobBuilderArray<float> depthArrayBuilder = builder.Allocate(ref ppBlob.m_depths, dataAmount);
        BlobBuilderArray<float> latArrayBuilder = builder.Allocate(ref ppBlob.m_lats, dataAmount);
        BlobBuilderArray<float> lonArrayBuilder = builder.Allocate(ref ppBlob.m_lons, dataAmount);

        int index = 0;
        for (int i = 0; i < numberParticles; i++)
        {
            for (int j = 0; j < entriesPerParticle; j++)
            {
                depthArrayBuilder[index] = (float)z.GetValue(i, j);
                latArrayBuilder[index] = (float)lat.GetValue(i, j);
                lonArrayBuilder[index] = (float)lon.GetValue(i, j);
                index++;
            }
        }

        BlobAssetReference<ParticlePropertiesBlob> blobAsset = builder.CreateBlobAssetReference<ParticlePropertiesBlob>(Allocator.Persistent);
		
		if(entityManager.HasComponent<ParticleSpawnData>(root))
        {
			entityManager.SetComponentData(root, new ParticleSpawnData
			{
				Value = blobAsset,
				m_entriesPerParticle = entriesPerParticle
			});
            entityManager.SetComponentEnabled<ParticleSpawnData>(root, true);
		}
        else
        {
		    entityManager.AddComponentData(root, new ParticleSpawnData
            {
                Value = blobAsset,
                m_entriesPerParticle = entriesPerParticle
            });
        }
        builder.Dispose();

		if (entityManager.HasComponent<ParticleTimingData>(root))
		{
			entityManager.SetComponentData(root, new ParticleTimingData
			{
				m_numberIndices = entriesPerParticle,
				m_timePerIndex = 0.5f //TODO: set this
			});
		}
		else
		{
			entityManager.AddComponentData(root, new ParticleTimingData
			{
				m_numberIndices = entriesPerParticle,
				m_timePerIndex = 0.5f //TODO: set this
			});
		}

        StartCoroutine(WaitForParticles());
    }

    public void UpdateParticleVisualizationSettingsData(int a_colourIndex, int a_darknessIndex)
    {
        // Get the active world and the EntityManager
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        // Create a query to get the Entity with the ParticleVisualizationSettingsData component
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
        if (!query.HasSingleton<ParticleVisualizationSettingsData>())
        {
            Debug.LogError("No 'ParticleVisualizationSettingsData' component found. Setting particle visualization settings data failed.");
            return;
        }
        Entity settingsEntity = query.GetSingletonEntity();

        if (entityManager.Exists(settingsEntity))
        {
            var settingsData = entityManager.GetComponentData<ParticleVisualizationSettingsData>(settingsEntity);

            settingsData.m_colourIndex = a_colourIndex;
            settingsData.m_darknessIndex = a_darknessIndex;

            entityManager.SetComponentData(settingsEntity, settingsData);
        }
    }

    /// <summary>
    /// This co-routine waits for the particles to be available in the Entities world and fires
    /// an event when they are loaded.
    /// </summary>
    IEnumerator WaitForParticles()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // Query for all entities that have a ParticleProperties component.
        var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ParticleProperties>());

        // Wait until the particles are available in the scene.
        while (query.IsEmpty)
        {
            yield return null;
        }

        float3 minPosition = new float3(float.MaxValue, float.MaxValue, float.MaxValue);
        float3 maxPosition = new float3(float.MinValue, float.MinValue, float.MinValue);

        var particleProperties = query.ToComponentDataArray<ParticleProperties>(Allocator.Temp);

        // Compute the current index to query based on the current time value of the timeline view
        // and the number of values in the particle properties.
        int index = m_MapInterface ? (int)(m_MapInterface.timelineTestValue * particleProperties[0].Value.Value.m_lons.Length) : 0;

        foreach (var p in particleProperties)
        {
            float3 pos = float3(p.Value.Value.m_lons[index], p.Value.Value.m_lats[index], p.Value.Value.m_depths[index]);
            minPosition = min(minPosition, pos);
            maxPosition = max(maxPosition, pos);
        }
        particleProperties.Dispose();

        m_onParticlesReadyEvent?.Invoke((minPosition + maxPosition) / 2.0f);
    }

    void FlushParticles()
    {
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
        if (!query.HasSingleton<ParticleVisualizationSettingsData>())
        {
            Debug.LogError("No 'DestroyParticlesTag' component found. Flushing particles failed.");
            return;
        }
		Entity root = query.GetSingletonEntity();
		if (entityManager.HasComponent<DestroyParticlesTag>(root))
        {
            entityManager.SetComponentEnabled<DestroyParticlesTag>(root, true);
        }
        else
			entityManager.AddComponentData(root, new DestroyParticlesTag());
	}
}

