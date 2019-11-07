using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Based on ARFoundation-samples script "PlaceOnPlane"
//
// Listens for touch events and performs an AR raycast from the screen touch point.
// AR raycasts will only hit detected trackables like feature points and planes.
//
// If a raycast hits a trackable, the placedPrefab is instantiated
// and moved to the hit position.
// 
// Modified 10/26/2019 by Ted Barnett for TimeWalk.org to enable switching between models
// 

[RequireComponent(typeof(ARRaycastManager))]

public class timewalkControllerAR : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    // Assign in the inspector
    public GameObject placedPrefab // The prefab to instantiate on touch.
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }
    public Slider rotationSlider;
    public Slider scaleSlider;

    // From timewalkController
    public static List<GameObject> myListObjects = new List<GameObject>(); // list where prefabs will be stored
    public static int currentObjectIndex = 0;
    public static int objectsListLength = 0;
    public static GameObject currentObject;
    private Text modelNameText;
    private Text debugText;
    private Renderer planeMeshRenderer;
    private string objectNameString;
    private bool runningOnDesktop;

    // Preserve the original and current orientation
    private float previousValue;
    private float previousValueScale;
    private GameObject timeWalkObject;
    private GameObject positionHolderObject;

    //private GameObject positionHolderObject; // an empty prefab used to private "parenting" to the objects

    // The object instantiated as a result of a successful raycast intersection with a plane.
    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();

        // Assign callbacks for sliders
        this.rotationSlider.onValueChanged.AddListener(this.OnRotationSliderChanged); // rotation slider callback
        this.previousValue = this.rotationSlider.value; // store current rotation slider value

        this.scaleSlider.onValueChanged.AddListener(this.OnScaleSliderChanged); // scale slider callback
        this.previousValueScale = this.scaleSlider.value; // store current scale slider value

        #if UNITY_EDITOR
            runningOnDesktop = true;
        #else
            runningOnDesktop = false;
        #endif
    }

    void Start()
    {
        modelNameText = GameObject.Find("Model Name").GetComponent<Text>();
        debugText = GameObject.Find("Debug Text").GetComponent<Text>();
        //planeMeshRenderer = GameObject.Find("AR Feathered Plane Fade").GetComponent<MeshRenderer>(); // TODO - Find and hide this plane

        // Set up list of all prefabs to cycle through...
        // NOTE: make sure all building prefabs are inside this folder: "Assets/Resources/Prefabs"

        Object[] subListObjects = Resources.LoadAll("Prefabs", typeof(GameObject));
        foreach (GameObject subListObject in subListObjects)
        {
            GameObject lo = (GameObject)subListObject;
            myListObjects.Add(lo);
            ++objectsListLength;
        }

        GameObject myObj = Instantiate(myListObjects[currentObjectIndex]) as GameObject;
        myObj.transform.parent = gameObject.transform; // Makes "myObj" a child of AR Session

        objectNameString = myObj.name;
        objectNameString = objectNameString.Substring(5); // strip off number in front of prefab name
        modelNameText.text = objectNameString.Replace("(Clone)", "");

        modelNameText.text = ""; // blank name until placed
        myObj.transform.gameObject.SetActive(false); // hide object at start (not yet placed)

        myObj.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value); // scale per current scale slider
        myObj.transform.Rotate(Vector3.down * rotationSlider.value * 360); // rotate per current rotation slider

        currentObject = myObj;

    }

    // GET TOUCH EVENTS (or Mouse Clicks)
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
        return false; // if not click or touch, then return "false"
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
                        spawnedObject = Instantiate(m_PlacedPrefab, new Vector3(2, 0, 0), Quaternion.identity);
                        currentObject.transform.parent = spawnedObject.gameObject.transform; // set as child of the spawned object
                        currentObject.transform.gameObject.SetActive(true); // show the object now that it is placed
                        // planeMeshRenderer.enabled = false; // TODO: Make plane rendered hide work
                        debugText.text = "position: " + spawnedObject.transform.position;
                        debugText.text = debugText.text + "\n" + "parent: " + spawnedObject.gameObject.name;
                        debugText.text = debugText.text + "\n" + "FIRST PLACEMENT";

                }
                else
                    {
                        spawnedObject.transform.position = new Vector3(1, 0, 0);
                        debugText.text = "position: " + spawnedObject.transform.position;
                        debugText.text = debugText.text + "\n" + "parent: " + spawnedObject.gameObject.name;
                }
            }
            }
        #endif

        // Return if not a valid AR "touch"
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) return;
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return; // ignore touches on UI (sliders, etc.)
            if (!TryGetTouchPosition(out Vector2 touchPosition)) return; // if no touch, then return from Update

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose; // Raycast hits are sorted by distance, so the first one will be the closest hit

            if (spawnedObject == null) // if the object has not been spawned yet
            {
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                currentObject.transform.parent = spawnedObject.gameObject.transform; // set as child of the spawned object
                currentObject.transform.gameObject.SetActive(true); // show the object now that it is placed
                debugText.text = "position: " + spawnedObject.transform.position;
                debugText.text = debugText.text + "\n" + "parent: " + spawnedObject.gameObject.name;
                debugText.text = debugText.text + "\n" + "FIRST PLACEMENT";
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                debugText.text = "position: " + spawnedObject.transform.position;
                debugText.text = debugText.text + "\n" + "parent: " + spawnedObject.gameObject.name;
            }
        }
    }

    // Instantiate next model in prefab list: incrementNumber = +1 for "next", -1 for "previous"
    public void SpawnNextObject(int incrementNumber)
    {
        Destroy(currentObject);

        currentObjectIndex = currentObjectIndex + incrementNumber;
        if (currentObjectIndex >= objectsListLength) currentObjectIndex = 0;
        if (currentObjectIndex < 0) currentObjectIndex = objectsListLength - 1;

        GameObject myObj = Instantiate(myListObjects[currentObjectIndex]) as GameObject;
        myObj.transform.gameObject.SetActive(true); // TODO: Is this necessary???

        //myObj.transform.parent = positionHolderObject.gameObject.transform; // set as child of the timewalkObject
        myObj.transform.parent = gameObject.transform; // original version
        myObj.transform.localScale = new Vector3(scaleSlider.value, scaleSlider.value, scaleSlider.value);
        myObj.transform.Rotate(Vector3.down * rotationSlider.value * 360);

        objectNameString = myObj.name;
        objectNameString = objectNameString.Substring(5); // strip off number in front of prefab name
        modelNameText.text = objectNameString.Replace("(Clone)", "");

        myObj.transform.position = transform.position; // Should we revert to this?

        currentObject = myObj;
    }

    // ROTATION CHANGE
    void OnRotationSliderChanged(float value)
    {
        // How much we've changed the rotation
        float delta = value - this.previousValue;
        currentObject.transform.Rotate(Vector3.down * delta * 360);

        // Set our previous value for the next change
        this.previousValue = value;
    }

    // SCALE CHANGE
    void OnScaleSliderChanged(float value)
    {

        //timeWalkObject = GameObject.Find("TimeWalkObject");

        // Set scale based on slider position
        currentObject.transform.localScale = new Vector3(value, value, value);

    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
