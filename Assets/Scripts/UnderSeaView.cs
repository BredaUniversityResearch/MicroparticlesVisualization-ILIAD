using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AbstractMap))]
public class UnderSeaView : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The cursor to use when hovering over the map.
    /// </summary>
    public Texture2D SeaViewCursor;

    /// <summary>
    /// The hotspot of the cursor.
    /// </summary>
    public Vector2 HotSpot;

    /// <summary>
    /// The map component.
    /// </summary>
    private AbstractMap map;

    void Start()
    {
        map = GetComponent<AbstractMap>();
        // This is required to get the pointer events to work on the map.
        //map.Terrain.SetElevationType(ElevationLayerType.TerrainWithElevation);
        //map.Terrain.EnableCollider(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("Zoom to under sea view.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(SeaViewCursor, HotSpot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
