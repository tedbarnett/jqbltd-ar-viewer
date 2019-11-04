using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;


    // Assign in the inspector
    private GameObject objectToRotate;
    public Slider rotationSlider;
    public Slider scaleSlider;

    // Preserve the original and current orientation
    private float previousValue;

    private Text debugText;
    private Text modelNameText;
    private string objectNameString;

    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    // The object instantiated as a result of a successful raycast intersection with a plane.
    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        debugText = GameObject.Find("Debug Text").GetComponent<Text>();
        modelNameText = GameObject.Find("Model Name").GetComponent<Text>();

        // Assign a callback for when the rotation slider changes
        this.rotationSlider.onValueChanged.AddListener(this.OnRotationSliderChanged); // rotation slider callback
        this.scaleSlider.onValueChanged.AddListener(this.OnScaleSliderChanged); // rotation slider callback

        // And current value
        this.previousValue = this.rotationSlider.value;
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                touchPosition = new Vector2(mousePosition.x, mousePosition.y);
                return true;
            }
        #else
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }
        #endif

        touchPosition = default;
        return false;
    }

    void Update()
    {

        // Return if clicking in UI area
        Touch touch; // per ARCore example (compare to below)

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return; // don't update if just touched or m_Content is null (not set yet)
        }

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                objectToRotate = spawnedObject;

                objectNameString = placedPrefab.name;
                objectNameString = objectNameString.Substring(5); // strip off number in front of prefab name
                modelNameText.text = objectNameString.Replace("(Clone)", "");

            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }


    void OnRotationSliderChanged(float value)
    {
        // How much we've changed
        float delta = value - this.previousValue;
        this.objectToRotate.transform.Rotate(Vector3.down * delta * 360);

        // Set our previous value for the next change
        this.previousValue = value;
    }



    // SCALE CHANGE
    void OnScaleSliderChanged(float value)
    {
        // Set scale based on slider position
        this.objectToRotate.transform.localScale = new Vector3(value, value, value);
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
