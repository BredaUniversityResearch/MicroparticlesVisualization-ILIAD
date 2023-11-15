using Mapbox.Unity.Map;
using Unity.Entities;
using UnityEngine;
using static Unity.Mathematics.math;

/// <summary>
/// This class interfaces with the AbstractMap in the main scene with the AbstractMapData in the Entities world.
/// </summary>
public class AbstractMapInterface : MonoBehaviour
{
    public AbstractMap map;
    [Range(0.1f, 100.0f)]
    public float particleScale = 1.0f;
    
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
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = entityManager.CreateEntityQuery(typeof(AbstractMapData));
        if (entityQuery.HasSingleton<AbstractMapData>())
        {
            var mapEntity = entityQuery.GetSingletonEntity();

            var abstractMapData = new AbstractMapData
            {
                scaleFactor = Mathf.Pow(2, map.InitialZoom - map.AbsoluteZoom),
                worldRelativeScale = map.WorldRelativeScale,
                centerMercator = double2(map.CenterMercator.x, map.CenterMercator.y),
                mapPosition = map.transform.position,
                mapScale = map.transform.localScale,
                mapRotation = map.transform.rotation,
                particleScale = particleScale
            };

            entityManager.SetComponentData(mapEntity, abstractMapData);
        }
    }
}
