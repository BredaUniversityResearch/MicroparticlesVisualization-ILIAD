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

            var abstractMapData = new AbstractMapData();

            abstractMapData.scaleFactor = Mathf.Pow(2, map.InitialZoom - map.AbsoluteZoom);
            abstractMapData.worldRelativeScale = map.WorldRelativeScale;
            abstractMapData.centerMercator = double2(map.CenterMercator.x, map.CenterMercator.y);
            abstractMapData.mapPosition = map.transform.position;
            abstractMapData.mapScale = map.transform.localScale;
            abstractMapData.mapRotation = map.transform.rotation;

            entityManager.SetComponentData(mapEntity, abstractMapData);
        }
    }
}
