using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class timewalkController : MonoBehaviour
{
    
    public static List<GameObject> myListObjects = new List<GameObject>(); // list where prefabs will be stored
    public static int currentObjectIndex = 0;
    public static int objectsListLength = 0;
    public static GameObject currentObject;
    private Text modelNameText;
    private Text debugText;
    private string objectNameString;


    void Start()
    {
        modelNameText = GameObject.Find("Model Name").GetComponent<Text>();
        debugText = GameObject.Find("Debug Text").GetComponent<Text>();

        // NOTE: make sure all building prefabs are inside this folder: "Assets/Resources/Prefabs"

        Object[] subListObjects = Resources.LoadAll("Prefabs", typeof(GameObject));
        foreach (GameObject subListObject in subListObjects)
        {
            GameObject lo = (GameObject)subListObject;
            myListObjects.Add(lo);
            ++objectsListLength;
        }
        GameObject myObj = Instantiate(myListObjects[currentObjectIndex]) as GameObject;
        myObj.transform.parent = gameObject.transform; // make instantiated object a child of the TimeWalkObject?
        objectNameString = myObj.name;
        objectNameString = objectNameString.Substring(5);
        modelNameText.text = objectNameString.Replace("(Clone)", "");
        // modelNameText.text = "Place model below"; // blank name until placed
        myObj.transform.gameObject.SetActive(true); // hide object at start (not yet placed)

        currentObject = myObj;

        debugText.text = "";
        //debugText.transform.gameObject.SetActive(false); // hide debugText until there is a message

        //audioData = GetComponent<AudioSource>();
        //audioData.Play(0);
    }

    public void NextPrefab(int incrementValue)
    {

        SpawnNextObject(incrementValue);
    }

    public void SpawnNextObject(int incrementNumber)
    {
        Destroy(currentObject);
        currentObjectIndex = currentObjectIndex + incrementNumber;
        if (currentObjectIndex >= objectsListLength)
        {
            currentObjectIndex = 0;
        }

        if (currentObjectIndex < 0)
        {
            currentObjectIndex = objectsListLength - 1;
        }

        GameObject myObj = Instantiate(myListObjects[currentObjectIndex]) as GameObject;

        myObj.transform.gameObject.SetActive(true); // Is this necessary???

        myObj.transform.parent = gameObject.transform;

        objectNameString = myObj.name;
        objectNameString = objectNameString.Substring(5);
        modelNameText.text = objectNameString.Replace("(Clone)", "");
        debugText.text = "New model: " + modelNameText.text;
        debugText.transform.gameObject.SetActive(false); // show debugText

        // myObj.transform.position = transform.position; // NO: instead we will use the object's default position

        currentObject = myObj;
        // debugText.text = "currentObjectIndex = " + currentObjectIndex;
    }

    void Update()

    {

    }


    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;

}
