using UnityEngine.EventSystems;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform FollowTransform;

    [SerializeField] private Camera cam;

    [Header("Camera Movement Values")]
    [SerializeField] private float normalSpeed = 0.2f;
    [SerializeField] private float fastSpeedMultiplier = 2f;
    [SerializeField] private float rotationAmount = 8f;
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float movementTime = 8f;

    [Header("Camera Movement Boundaries")]
    [SerializeField] private Vector2 minPos = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxPos = new Vector2(70f, 70f);
    [SerializeField] private Vector2 zoomRange = new Vector2(40f, 175f);
    
    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private Vector3 _newZoom;
    private float _movementSpeed;

    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        _newPosition = transform.position;
        _newRotation = transform.rotation;
        _newZoom = cam.transform.localPosition;
    }
    private void Update()
    {
        // Преследование объекта (на объекте должен быть скрипт с методом OnMouseDown, 
        // который установит followTransform через singleton своим transform'ом)
        if (FollowTransform != null)
        {
            transform.position = FollowTransform.position;
        }
        else
        {
            HandleMouseInput();
            HandleMovementInput();
        }

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            FollowTransform = null;
        }*/
    }

    void HandleMouseInput()
    {
        if (!SceneController.IsMouseOverUI())
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                _newZoom -= cam.transform.localPosition.normalized *
                    Input.mouseScrollDelta.y * zoomAmount;
                _newZoom.y = Mathf.Clamp(_newZoom.y, zoomRange.x, zoomRange.y);
                _newZoom.z = Mathf.Clamp(_newZoom.z, -zoomRange.y, -zoomRange.x);

            }

            // Перемещение камеры с помощью перетаскивания с зажатой ЛКМ
            if (Input.GetMouseButtonDown(0))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    _dragStartPosition = ray.GetPoint(entry);
                }
            }
            if (Input.GetMouseButton(0))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    _dragCurrentPosition = ray.GetPoint(entry);

                    _newPosition = transform.position +
                        _dragStartPosition - _dragCurrentPosition;
                }
            }

            // Вращение камеры с помощью СКМ (зажимания колесика)
            if (Input.GetMouseButtonDown(2))
            {
                _rotateStartPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                _rotateCurrentPosition = Input.mousePosition;

                Vector3 difference = _rotateStartPosition - _rotateCurrentPosition;

                _rotateStartPosition = _rotateCurrentPosition;

                _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
            }
        }
    }
    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _movementSpeed = normalSpeed * fastSpeedMultiplier;
        }
        else
        {
            _movementSpeed = normalSpeed;
        }

        _newPosition += transform.forward * Input.GetAxisRaw("Vertical") * _movementSpeed;
        _newPosition += transform.right * Input.GetAxisRaw("Horizontal") * _movementSpeed;

        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            _newZoom += cam.transform.localPosition.normalized * zoomAmount * .2f;
        }
        if (Input.GetKey(KeyCode.F))
        {
            _newZoom -= cam.transform.localPosition.normalized * zoomAmount * .2f;
        }

        _newPosition.x = Mathf.Clamp(_newPosition.x, minPos.x, maxPos.x);
        _newPosition.z = Mathf.Clamp(_newPosition.z, minPos.y, maxPos.y);
        _newZoom.y = Mathf.Clamp(_newZoom.y, zoomRange.x, zoomRange.y);
        _newZoom.z = Mathf.Clamp(_newZoom.z, -zoomRange.y, -zoomRange.x);

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * movementTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, _newZoom, Time.deltaTime * movementTime);
    }
}
