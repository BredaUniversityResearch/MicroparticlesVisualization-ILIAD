using System;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CesiumOriginShift))]
[RequireComponent(typeof(CesiumGlobeAnchor))]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public enum ViewMode
    {
        UnderWater,
        TopDown
    }

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
    public AnimationCurve MoveSpeed;

    /// <summary>
    /// The amount of damping to apply to the velocity vector each frame.
    /// 0 is no damping, 1 is full damping.
    /// </summary>
    [Range(0, 1)]
    public float VelocityDamping = 0.8f;

    #endregion

    #region Input Actions

    private InputAction shiftAction;
    private InputAction ctrlAction;
    private InputAction altAction;
    private InputAction lmbAction; // Left mouse button.
    private InputAction rmbAction; // Right mouse button.
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction flyAction;
    private InputAction zoomAction;
    private InputAction resetAction;
    private InputAction stopAction;
    private InputAction verticalAction;
    private InputAction switchViewAction;

    #endregion

    #region Private Fields

    [SerializeField]
    [Min(0.0f)]
    private float dynamicClippingPlanesMinHeight = 10000.0f;

    // The georeference object on the parent.
    private CesiumGeoreference georeference;
    private CesiumGlobeAnchor globeAnchor;
    private double3 initialPosition;
    private quaternion initialRotation;

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
    private Vector3 velocity;
    private double previousHeight;

    /// <summary>
    /// Use a character controller to avoid clipping through the terrain.
    /// </summary>
    private CharacterController characterController;

    private ViewMode currentViewMode = ViewMode.TopDown;

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
        flyAction = map.FindAction("fly");
        zoomAction = map.FindAction("zoom");
        resetAction = map.FindAction("reset");
        stopAction = map.FindAction("stop");
        verticalAction = map.FindAction("vertical");
        switchViewAction = map.FindAction("switchView");

        shiftAction.Enable();
        ctrlAction.Enable();
        altAction.Enable();
        lmbAction.Enable();
        rmbAction.Enable();
        lookAction.Enable();
        moveAction.Enable();
        flyAction.Enable();
        zoomAction.Enable();
        resetAction.Enable();
        stopAction.Enable();
        verticalAction.Enable();
        switchViewAction.Enable();
    }

    void InitialiseController()
    {
        if (GetComponent<CharacterController>() != null)
        {
            Debug.LogWarning(
                "A CharacterController component was manually " +
                "added to the CesiumCameraController's game object. " +
                "This may interfere with the CesiumCameraController's movement.");

            characterController = GetComponent<CharacterController>();
        }
        else
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.hideFlags = HideFlags.HideInInspector;
        }

        characterController.radius = 1.0f;
        characterController.height = 1.0f;
        characterController.center = Vector3.zero;
        characterController.detectCollisions = true;
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
        initialPosition = globeAnchor.longitudeLatitudeHeight;
        initialRotation = globeAnchor.rotationEastUpNorth;

        ConfigureInputs();
        InitialiseController();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Cast a ray from the mouse to the earth and return the point that was hit in Unity world coordinates.
    /// </summary>
    /// <param name="p">The point in Unity world coordinates.</param>
    /// <returns>`true` if the globe terrain was hit, `false` otherwise.</returns>
    public bool GetMousePointOnGlobe(out Vector3 p)
    {
        Vector3 screen = Mouse.current.position.value;
        screen.z = camera.farClipPlane;
        Vector3 end = camera.ScreenToWorldPoint(screen);

        // TODO: Check the terrain tileset was hit?
        if (Physics.Linecast(camera.transform.position, end, out var hitInfo))
        {
            p = hitInfo.point;
            return true;
        }

        p = end;
        return false;
    }

    /// <summary>
    /// Get the height of the camera (in meters) above the ellipsoid of the world.
    /// Note: Clamp to 0 to avoid negative heights.
    /// </summary>
    private float Height => Mathf.Max(0.0f, (float)globeAnchor.longitudeLatitudeHeight.z);

    void Move(Vector3 move)
    {
        if (lmbAction.WasPressedThisFrame())
        {
            GetMousePointOnGlobe(out previousMousePosition);
        }
        else if (lmbAction.IsPressed())
        {
            GetMousePointOnGlobe(out var currentMousePosition);

            Vector3 delta = previousMousePosition - currentMousePosition;
            delta.y = 0;
            //camera.transform.Translate(delta, Space.World);
            characterController.Move(delta);
            velocity = delta / Time.deltaTime;
            previousMousePosition = currentMousePosition;
        }
        else if (move.sqrMagnitude > 0.0f)
        {
            velocity = Vector3.zero;

            Vector3 forwardDirection = Vector3.Cross(camera.transform.right, Vector3.up).normalized;
            Vector3 moveDirection = camera.transform.right * move.x + forwardDirection * move.z;
            float moveSpeed = MoveSpeed.Evaluate(Height);
            //camera.transform.Translate(moveDirection * moveSpeed, Space.World);
            characterController.Move(moveDirection * moveSpeed);
        }
        else
        {
            // Propagate the current velocity.
            //camera.transform.Translate(velocity * Time.deltaTime, Space.World);
            characterController.Move(velocity * Time.deltaTime);
        }

    }

    /// <summary>
    /// Translation about the view's local forward vector.
    /// </summary>
    /// <param name="zoom">The zoom amount.</param>
    void Zoom(float zoom)
    {
        float speed = MoveSpeed.Evaluate(Height);
        //camera.transform.Translate(Vector3.forward * zoom * speed, Space.Self);
        characterController.Move(transform.TransformDirection(Vector3.forward * zoom * speed));
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

    /// <summary>
    /// This method uses a "flight simulator" model for the camera. Movement is in the camera's local coordinate space
    /// instead of global coordinates (that the Move function uses).
    /// </summary>
    /// <param name="move"></param>
    void Fly(Vector3 move)
    {
        float moveSpeed = MoveSpeed.Evaluate(Height);
        // camera.transform.Translate(move * moveSpeed, Space.Self);
        characterController.Move(transform.TransformDirection(move * moveSpeed));
        velocity = move * moveSpeed;
    }


    // TODO: This function currently doesn't work. Rotating around a point in Unity coordinates will
    // not work due to the CesiumGlobeAnchor that adjusts the Georeference and reverts any changes to the 
    // camera's world position. Probably need to perform the rotation in ECEF coordinates and transform the 
    // globe anchor directly.
    void Rotate(float deltaPitch, float deltaYaw)
    {
        if (lmbAction.WasPerformedThisFrame())
        {
            GetMousePointOnGlobe(out previousMousePosition);
        }
        else if (lmbAction.IsPressed())
        {
            float dX = deltaPitch * LookSpeed * Time.smoothDeltaTime;
            float dY = deltaYaw * LookSpeed * Time.smoothDeltaTime;

            camera.transform.RotateAround(previousMousePosition, Vector3.up, dY);
            camera.transform.RotateAround(previousMousePosition, Vector3.right, dX);
        }
        else
        {
            // TODO: Raycast based on the camera's forward vector and rotate about the point on the terrain that the camera is looking at.
        }
    }

    /// <summary>
    /// Switch the view mode between under water or top-down view modes.
    /// </summary>
    void SwitchView()
    {
        Vector3 clickedPoint;

        switch (currentViewMode)
        {
            case ViewMode.TopDown:
                if (GetMousePointOnGlobe(out clickedPoint))
                {
                    Debug.Log("Switching view mode to underwater.");

                    // Store the current height of the camera to restore it when going back to top-down view.
                    previousHeight = globeAnchor.longitudeLatitudeHeight.z;

                    camera.transform.position = clickedPoint + Vector3.up * 10.0f; // 10 units above the clicked point.

                    var eulerAngles = camera.transform.localEulerAngles;
                    camera.transform.localEulerAngles = new Vector3(0.0f, eulerAngles.y, eulerAngles.z); // Remove X rotation.
                    currentViewMode = ViewMode.UnderWater;
                }
                break;
            case ViewMode.UnderWater:
                // if (GetMousePointOnGlobe(out clickedPoint))
                {
                    Debug.Log("Switching view mode to top-down.");
                    var llh = globeAnchor.longitudeLatitudeHeight;
                    llh.z = previousHeight; // Restore previous height.
                    globeAnchor.longitudeLatitudeHeight = llh;

                    var eulerAngles = camera.transform.localEulerAngles;
                    eulerAngles.x = 90.0f; // Look down.
                    camera.transform.localEulerAngles = eulerAngles;

                    currentViewMode = ViewMode.TopDown;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Ignore any input if the cursor is over a UI element.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        bool shift = shiftAction.IsPressed();
        bool ctrl = ctrlAction.IsPressed();
        bool alt = altAction.IsPressed();
        bool lmb = lmbAction.IsPressed();
        bool rmb = rmbAction.IsPressed();
        bool switchView = switchViewAction.triggered;

        Vector2 lookDelta = lookAction.ReadValue<Vector2>();
        Vector2 moveDelta = moveAction.ReadValue<Vector2>();
        Vector2 flyDelta = flyAction.ReadValue<Vector2>();
        float zoom = zoomAction.ReadValue<Vector2>().y;
        float vertical = verticalAction.ReadValue<float>();

        if (stopAction.WasPressedThisFrame() || resetAction.WasPerformedThisFrame())
        {
            velocity = Vector3.zero;
        }

        if (switchView)
        {
            SwitchView();
        }
        else if (rmb)
        {
            if (EnableMovement)
            {
                Fly(new Vector3(moveDelta.x, vertical, moveDelta.y));
            }

            if (EnableRotation)
            {
                Look(flyDelta.y, flyDelta.x);
            }
        }
        //else if (shift)
        //{
        //    if (EnableRotation)
        //    {
        //        // TODO: Fix this function. See TODO comment on the function.
        //        // Rotate(lookDelta.y, lookDelta.x);
        //    }
        //}
        else if (lmb && ctrl)
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
                Move(new Vector3(moveDelta.x, vertical, moveDelta.y));
                Zoom(zoom);
            }
        }

        if (EnableDynamicClippingPlanes)
        {
            UpdateClippingPlanes();
        }

        // Apply damping.
        velocity *= Mathf.Pow(1.0f - VelocityDamping, Time.deltaTime);
    }

    void LateUpdate()
    {
        if (resetAction.WasPressedThisFrame())
        {
            globeAnchor.longitudeLatitudeHeight = initialPosition;
            globeAnchor.rotationEastUpNorth = initialRotation;
            currentViewMode = ViewMode.TopDown;
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

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(previousMousePosition, 100);
    }
#endif
}
