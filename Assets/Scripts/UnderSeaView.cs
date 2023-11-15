using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

[RequireComponent(typeof(AbstractMap), typeof(MeshCollider))]
public class UnderSeaView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler, IDragHandler
{
    enum ViewMode
    {
        _2D,
        _3D
    }

    /// <summary>
    /// Current view mode.
    /// </summary>
    private ViewMode viewMode = ViewMode._2D;

    /// <summary>
    /// The top-down camera looking at the map.
    /// </summary>
    public Camera MapCamera;

    /// <summary>
    /// The under water camera.
    /// </summary>
    public Camera UnderSeaCamera;

    /// <summary>
    /// Pitch speed (degree/pixel).
    /// </summary>
    public float PitchSpeed = 1.0f;

    /// <summary>
    /// Yaw speed (degrees/pixel).
    /// </summary>
    public float YawSpeed = 1.0f;

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

    private float pitch = 0;
    private float yaw = 0;

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

        MapCamera.enabled = true;
        UnderSeaCamera.enabled = false;
        viewMode = ViewMode._2D;

        pitch = UnderSeaCamera.transform.localEulerAngles.y;
        yaw = UnderSeaCamera.transform.localEulerAngles.x;

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
                case ViewMode._2D:
                    MapCamera.enabled = false;
                    UnderSeaCamera.enabled = true;
                    viewMode = ViewMode._3D;
                    break;
                case ViewMode._3D:
                    UnderSeaCamera.enabled = false;
                    MapCamera.enabled = true;
                    viewMode = ViewMode._2D;
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

#if UNITY_EDITOR
    // For debugging purposes only.
    private Vector3 debugCursor;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugCursor, 1);
    }
#endif

    public void OnPointerMove(PointerEventData eventData)
    {
        if (viewMode == ViewMode._2D)
        {
            Vector3 screenPoint = eventData.position;
            screenPoint.z = MapCamera.transform.localPosition.y;

            var worldPoint = MapCamera.ScreenToWorldPoint(screenPoint);

            UnderSeaCamera.transform.localPosition = new Vector3(worldPoint.x, UnderSeaCamera.transform.localPosition.y, worldPoint.z);

#if UNITY_EDITOR
            debugCursor = worldPoint;
#endif
        }
    }

    public void RotateCamera3D(PointerEventData eventData, Camera camera)
    {
        if (eventData.dragging && eventData.button == InputButton.Right)
        {

            pitch -= eventData.delta.y * PitchSpeed;
            yaw += eventData.delta.x * YawSpeed;

            var rotation = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);

            camera.transform.localRotation = rotation;
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (viewMode == ViewMode._3D)
        {
            RotateCamera3D(eventData, UnderSeaCamera);
        }
    }
}
