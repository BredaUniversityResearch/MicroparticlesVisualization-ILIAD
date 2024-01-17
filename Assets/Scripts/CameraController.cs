using System;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CesiumOriginShift))]
[RequireComponent(typeof(CesiumGlobeAnchor))]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    #region Public Properties

    /// <summary>
    /// The input action map that defines the actions and bindings.
    /// </summary>
    public InputActionAsset ActionMap;

    /// <summary>
    /// Enable movement on this camera controller.
    /// </summary>
    public bool EnableMovement = true;

    /// <summary>
    /// Enable rotation on this camera controller.
    /// </summary>
    public bool EnableRotation = true;

    /// <summary>
    /// How fast to rotate the camera when looking around.
    /// </summary>
    public float LookSpeed = 10.0f;

    /// <summary>
    /// Automatically adjust the camera's clipping planes so that the globe
    /// will not be clipped when viewed from far away.
    /// </summary>
    public bool EnableDynamicClippingPlanes;

    /// <summary>
    /// The height to start dynamically adjusting the camera's clipping planes. 
    /// Below this height, the clipping planes will be set to their initial values.
    /// </summary>
    public float DynamicClippingPlanesMinHeight
    {
        get => dynamicClippingPlanesMinHeight;
        set => dynamicClippingPlanesMinHeight = Mathf.Max(value, 0.0f);
    }

    /// <summary>
    /// A curve that is used to determine the movement speed of the camera
    /// based on the height of the camera. The higher the height of the camera,
    /// the faster it should move. 
    /// </summary>
    public AnimationCurve moveSpeed;

    #endregion

    #region Input Actions

    private InputAction shiftAction;
    private InputAction ctrlAction;
    private InputAction altAction;
    private InputAction lmbAction;
    private InputAction rmbAction; // Right mouse button.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction resetAction;

    #endregion

    #region Private Fields

    [SerializeField]
    [Min(0.0f)]
    private float dynamicClippingPlanesMinHeight = 10000.0f;

    // The georeference object on the parent.
    private CesiumGeoreference georeference;
    private CesiumGlobeAnchor globeAnchor;
    private double4x4 initialLocalToGlobeFixedMatrix;

    private new Camera camera;
    private float initialNearClipPlane;
    private float initialFarClipPlane;

    // If the near clip gets too large, Unity will throw errors. Keeping it 
    // at this value works fine even when the far clip plane gets large.
    private float maximumNearClipPlane = 1000.0f;
    private float maximumFarClipPlane = 500000000.0f;

    // The maximum ratio that the far clip plane is allowed to be larger
    // than the near clip plane. The near clip plane is set so that this
    // ratio is never exceeded.
    private float maximumNearToFarRatio = 100000.0f;

    private Vector3 previousMousePosition;

    #endregion


    void ConfigureInputs()
    {
        if (ActionMap == null)
        {
            throw new ArgumentNullException(nameof(ActionMap));
        }

        InputActionMap map = ActionMap.FindActionMap("CameraController");
        if (map == null)
        {
            throw new ArgumentNullException("CameraController");
        }

        shiftAction = map.FindAction("shift");
        ctrlAction = map.FindAction("ctrl");
        altAction = map.FindAction("alt");
        lmbAction = map.FindAction("lmb");
        rmbAction = map.FindAction("rmb");
        lookAction = map.FindAction("look");
        moveAction = map.FindAction("move");
        zoomAction = map.FindAction("zoom");
        resetAction = map.FindAction("reset");

        shiftAction.Enable();
        ctrlAction.Enable();
        altAction.Enable();
        lmbAction.Enable();
        rmbAction.Enable();
        lookAction.Enable();
        moveAction.Enable();
        zoomAction.Enable();
        resetAction.Enable();
    }

    void Awake()
    {
        georeference = GetComponentInParent<CesiumGeoreference>();
        if (georeference == null)
        {
            Debug.LogError(
                "CesiumCameraController must be nested under a game object " +
                "with a CesiumGeoreference.");
        }

        // CesiumOriginShift will add a CesiumGlobeAnchor automatically.
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        camera = GetComponent<Camera>();
        initialNearClipPlane = camera.nearClipPlane;
        initialFarClipPlane = camera.farClipPlane;
        initialLocalToGlobeFixedMatrix = globeAnchor.localToGlobeFixedMatrix;

        ConfigureInputs();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Cast a ray from the mouse to the earth and return the point that was hit in Unity world coordinates.
    /// </summary>
    /// <param name="p">The point in Unity world coordinates.</param>
    /// <returns>`true` if the globe was hit, `false` otherwise.</returns>
    public bool GetMousePointOnGlobe(out Vector3 p)
    {
        p = Vector3.zero;

        Vector3 screen = Mouse.current.position.value;
        screen.z = camera.farClipPlane;
        Vector3 end = camera.ScreenToWorldPoint(screen);

        if (Physics.Linecast(camera.transform.position, end, out var hitInfo))
        {
            p = hitInfo.point;
            return true;
        }
        
        return false;
    }

    void Move(Vector3 move)
    {
        if (lmbAction.WasPressedThisFrame())
        {
            GetMousePointOnGlobe(out previousMousePosition);
        }
        else if (lmbAction.IsPressed())
        {
            if (GetMousePointOnGlobe(out var currentMousePosition))
            {
                camera.transform.Translate(previousMousePosition - currentMousePosition, Space.World);
                previousMousePosition = currentMousePosition;
            }
        }
    }

    /// <summary>
    /// Rotate around the camera's pivot point.
    /// </summary>
    /// <param name="deltaPitch">The change in pitch (rotation about the local X-axis).</param>
    /// <param name="deltaYaw">The change in yaw (rotation about the local Y-axis).</param>
    void Look(float deltaPitch, float deltaYaw)
    {
        float dX = deltaPitch * LookSpeed * Time.smoothDeltaTime;
        float dY = deltaYaw * LookSpeed * Time.smoothDeltaTime;

        float rotY = transform.localEulerAngles.y - dY;
        float rotX = transform.localEulerAngles.x;
        if (rotX <= 90.0f)
        {
            rotX += 360.0f;
        }
        rotX = Mathf.Clamp(rotX + dX, 270.0f, 450.0f);

        transform.localEulerAngles = new Vector3(rotX, rotY, transform.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        bool shift = shiftAction.IsPressed();
        bool ctrl = ctrlAction.IsPressed();
        bool alt = altAction.IsPressed();
        bool lmb = lmbAction.IsPressed();
        bool rmb = rmbAction.IsPressed();

        Vector2 lookDelta = lookAction.ReadValue<Vector2>();
        Vector2 moveDelta = moveAction.ReadValue<Vector2>();
        float zoom = zoomAction.ReadValue<Vector2>().y;

        if (resetAction.WasPressedThisFrame())
        {
            globeAnchor.localToGlobeFixedMatrix = initialLocalToGlobeFixedMatrix;
        }

        if (ctrl)
        {
            if (EnableRotation)
            {
                Look(lookDelta.y, lookDelta.x);
            }
        }
        else
        {
            if (EnableMovement)
            {
                Move(new Vector3(moveDelta.x, zoom, moveDelta.y));
            }
        }

        if (EnableDynamicClippingPlanes)
        {
            UpdateClippingPlanes();
        }
    }

    private bool RaycastTowardsEarthCenter(out float hitDistance)
    {
        double3 center = georeference.TransformEarthCenteredEarthFixedPositionToUnity(new double3(0.0));

        RaycastHit hitInfo;
        if (Physics.Linecast(this.transform.position, (float3)center, out hitInfo))
        {
            hitDistance = Vector3.Distance(this.transform.position, hitInfo.point);
            return true;
        }

        hitDistance = 0.0f;
        return false;
    }

    void UpdateClippingPlanes()
    {
        if (RaycastTowardsEarthCenter(out var height))
        {
            float nearClipPlane = initialNearClipPlane;
            float farClipPlane = initialFarClipPlane;

            if (height >= dynamicClippingPlanesMinHeight)
            {
                farClipPlane = height + (float)(2.0 * CesiumWgs84Ellipsoid.GetMaximumRadius());
                farClipPlane = Mathf.Min(farClipPlane, maximumFarClipPlane);
                float farClipRatio = farClipPlane / maximumNearToFarRatio;

                if (farClipRatio > nearClipPlane)
                {
                    nearClipPlane = Mathf.Min(farClipRatio, maximumNearClipPlane);
                }
            }

            camera.nearClipPlane = nearClipPlane;
            camera.farClipPlane = farClipPlane;
        }
    }
}
