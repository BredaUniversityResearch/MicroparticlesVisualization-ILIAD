using System;
using System.Collections;
using Microsoft.Research.Science.Data;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class DataLoader : MonoBehaviour
{
    static DataLoader m_instance;

    private int m_nextRequestIndex = -1;

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

    public void LoadJsonFile(string a_filePath, Action<bool> a_completedCallback)
    {
        string path = "file:///" + a_filePath;
        Debug.Log("Loading JSON from path: " + path);
        StartCoroutine(LoadJSON(UnityWebRequest.Get(path), a_completedCallback, ++m_nextRequestIndex));
    }

    public void LoadJsonURL(string a_url, Action<bool> a_completedCallback)
    {
        Debug.Log("Loading JSON from URL: " + a_url);
        StartCoroutine(LoadJSON(UnityWebRequest.Get(a_url), a_completedCallback, ++m_nextRequestIndex));
    }

    IEnumerator LoadJSON(UnityWebRequest a_request, Action<bool> a_completedCallback, int a_requestIndex)
    {
        yield return a_request.SendWebRequest();

        if (a_request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fetching JSON file failed, message: " + a_request.error);
            a_completedCallback.Invoke(false);
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
                a_completedCallback?.Invoke(true);
            }
            else
                a_completedCallback?.Invoke(false);
        }

    }

    public void LoadNCDFFile(string a_filePath, Action<bool> a_completedCallback)
    {
        bool success = false;
        m_nextRequestIndex++;
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
		}
    }

    public void LoadNCDFURL(string a_url, Action<bool> a_completedCallback)
    {
        m_nextRequestIndex++;
        //TODO
    }

    public void SetParticleSpawnData(GeoVizData a_data)
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
        entityManager.AddComponentData(root, new ParticleSpawnData
        {
            Value = blobAsset,
            m_entriesPerParticle = a_data.entries_per_particle
        });
        builder.Dispose();
        entityManager.AddComponentData(root, new ParticleTimingData
        {
            m_numberIndices = a_data.entries_per_particle,
            m_timePerIndex = 0.5f //TODO: set this
        });
    }

    public void SetParticleSpawnData(DataSet a_data)
    {
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

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
        if (!query.HasSingleton<ParticleVisualizationSettingsData>())
            return;
        Entity root = query.GetSingletonEntity();

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
        entityManager.AddComponentData(root, new ParticleSpawnData
        {
            Value = blobAsset,
            m_entriesPerParticle = entriesPerParticle
        });
        builder.Dispose();
        entityManager.AddComponentData(root, new ParticleTimingData
        {
            m_numberIndices = entriesPerParticle,
            m_timePerIndex = 0.5f //TODO: set this
        });
    }
}

