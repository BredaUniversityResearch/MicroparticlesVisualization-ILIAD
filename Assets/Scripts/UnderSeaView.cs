using System;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AbstractMap), typeof(MeshCollider))]
public class UnderSeaView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
{
    enum ViewMode
    {
        Map,
        UnderSea
    }

    private ViewMode viewMode = ViewMode.Map;

    /// <summary>
    /// The top-down camera looking at the map.
    /// </summary>
    public Camera mapCamera;

    /// <summary>
    /// The under water camera.
    /// </summary>
    public Camera underSeaCamera;

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

    /// <summary>
    /// Mesh collider for detecting pointer events.
    /// </summary>
    private MeshCollider meshCollider;

    /// <summary>
    /// Update the collider that is used for detecting pointer events.
    /// </summary>
    void UpdateMeshCollider()
    {
        var meshes = map.gameObject.GetComponentsInChildren<MeshFilter>();

        var combine = new CombineInstance[meshes.Length];
        for (int i = 0; i < meshes.Length; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            combine[i].transform = meshes[i].transform.parent.localToWorldMatrix.inverse * meshes[i].transform.localToWorldMatrix;  // Matrix4x4.TRS(meshes[i].transform.localPosition, meshes[i].transform.localRotation, meshes[i].transform.localScale);
        }

        var mesh = meshCollider.sharedMesh ?? new Mesh();

        mesh.CombineMeshes(combine);

        meshCollider.sharedMesh = mesh;
    }

    void Start()
    {
        map = GetComponent<AbstractMap>();
        meshCollider = GetComponent<MeshCollider>();

        mapCamera.enabled = true;
        underSeaCamera.enabled = false;
        viewMode = ViewMode.Map;

        UpdateMeshCollider();

        map.TileProvider.ExtentChanged += TileProvider_ExtentChanged;
    }

    private void TileProvider_ExtentChanged(object sender, ExtentArgs e)
    {
        UpdateMeshCollider();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            switch (viewMode)
            {
                case ViewMode.Map:
                    mapCamera.enabled = false;
                    underSeaCamera.enabled = true;
                    viewMode = ViewMode.UnderSea;
                    break;
                case ViewMode.UnderSea:
                    underSeaCamera.enabled = false;
                    mapCamera.enabled = true;
                    viewMode = ViewMode.Map;
                    break;
            }
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

    private Vector3 debugCursor;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugCursor, 1);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (viewMode == ViewMode.Map)
        {
            Vector3 screenPoint = eventData.position;
            screenPoint.z = mapCamera.transform.localPosition.y;

            var worldPoint = mapCamera.ScreenToWorldPoint(screenPoint);

            underSeaCamera.transform.localPosition = new Vector3(worldPoint.x, underSeaCamera.transform.localPosition.y, worldPoint.z);
            underSeaCamera.transform.LookAt(Vector3.zero); // TODO: Compute the position of the particle cluster.

            debugCursor = worldPoint;
        }
    }
}
