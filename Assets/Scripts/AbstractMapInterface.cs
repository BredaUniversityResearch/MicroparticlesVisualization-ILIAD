using CesiumForUnity;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// This class interfaces with the AbstractMap in the main scene with the AbstractMapData in the Entities world.
/// </summary>
public class AbstractMapInterface : MonoBehaviour
{
    public CesiumGeoreference Georeference;
    public CesiumGlobeAnchor CameraAnchor;

    [Range(0, 1)]
    public float timelineTestValue;

    [Range(0, 1)]
    public float stepRate = 0.1f;
    public bool play;

    // The size that the particle should appear on screen.
    [Range(0, 1)]
    public float particleSize;
    
    // Start is called before the first frame update
    void Start()
    {
        // Create a Singleton Entity for the AbstractMapData component.
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<AbstractMapData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            timelineTestValue = (timelineTestValue + stepRate * Time.deltaTime) % 1.0f;
        }

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = entityManager.CreateEntityQuery(typeof(AbstractMapData));
        if (entityQuery.HasSingleton<AbstractMapData>())
        {
            var mapEntity = entityQuery.GetSingletonEntity();

            var abstractMapData = new AbstractMapData
            {
                ECEFMatrix = Georeference.ecefToLocalMatrix,
                cameraPosition = Camera.main.transform.position,
                cameraHeight = (float)CameraAnchor.longitudeLatitudeHeight.z,
                cameraFoV =  Camera.main.fieldOfView * Mathf.Deg2Rad,
                timelineValue = timelineTestValue,
                particleSize = particleSize
            };

            entityManager.SetComponentData(mapEntity, abstractMapData);
        }
    }
}
