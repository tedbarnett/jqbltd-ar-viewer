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
public class TimeWalkPlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;


    // Assign in the inspector
    private GameObject objectToModify;
    private ParticleSystem fireworksParticleSystem;
    public Slider rotationSlider;
    public Slider scaleSlider;
    public Button fireworksButton;

    // Preserve the original and current orientation
    private float previousValue;
    private Text debugText;
    private Text scaleText;
    private Text modelNameText;
    public string displayName;

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
        scaleText = GameObject.Find("Scaling Ratio Text").GetComponent<Text>();
        modelNameText = GameObject.Find("Model Name").GetComponent<Text>();

        // Assign a callback for when the rotation slider changes
        this.rotationSlider.onValueChanged.AddListener(this.OnRotationSliderChanged); // rotation slider callback
        this.previousValue = this.rotationSlider.value;
        this.scaleSlider.onValueChanged.AddListener(this.OnScaleSliderChanged); // rotation slider callback
        this.fireworksButton.onClick.AddListener(this.LaunchFireworks); // fireworks launch callback

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

        #if UNITY_EDITOR
            if (Input.GetMouseButton(0)) // mouse click outside UI
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    var mousePosition = Input.mousePosition;
                    var clickPosition = new Vector2(mousePosition.x, mousePosition.y);
                    if (spawnedObject == null) // if the object has not been spawned yet, then spawn it at origin
                    {
                        spawnedObject = Instantiate(m_PlacedPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        //modelNameText.text = ModelNameFix(placedPrefab.name);
                        modelNameText.text = displayName;
                }
                    else
                    {
                        spawnedObject.transform.position = new Vector3(0, 0, 0);
                    }
                }
            }
        #endif

        // Return if clicking in UI area
        Touch touch; // per ARCore example (compare to below)

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) return;
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;



        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose; // Raycast hits are sorted by distance, so the first one will be the closest hit.

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                //modelNameText.text = ModelNameFix(placedPrefab.name);
                modelNameText.text = displayName;

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
        this.spawnedObject.transform.Rotate(Vector3.down * delta * 360);

        // Set our previous value for the next change
        this.previousValue = value;
    }

    void OnScaleSliderChanged(float value)
    {
        // Set scale based on slider position
        float scaleValue = ScaleConvert(value);
        double scaleRatio = System.Math.Round(100 / scaleValue, 1);
        if (scaleRatio > 10) scaleRatio = System.Math.Round(scaleRatio, 0);
        scaleText.text = "Scale = 1 : " + scaleRatio;
        this.spawnedObject.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }

    // Convert Prefab name to Model Name String
    string ModelNameFix(string originalName)
    {
        string ret;
        ret = originalName.Substring(5); // strip off number in front of prefab name
        ret = ret.Replace("(Clone)", "");
        return ret;
    }

    // Convert Scale Slider to Scale Value
    float ScaleConvert(float value)
    {
        float ret;
        ret = Mathf.Pow(10, (value - 1));
        return ret;
    }

    void LaunchFireworks()
    {
        fireworksParticleSystem = GameObject.Find("Rocket particles").GetComponent<ParticleSystem>();
        fireworksParticleSystem.Play();

    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
