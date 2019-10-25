using UnityEngine;
using UnityEngine.UI;

public class RotateWithSlider : MonoBehaviour
{
    // Assign in the inspector
    private GameObject objectToRotate;
    public Slider slider;

    // Preserve the original and current orientation
    private float previousValue;

    void Awake()
    {
        // Assign a callback for when this slider changes
        this.slider.onValueChanged.AddListener(this.OnSliderChanged);

        // And current value
        this.previousValue = this.slider.value;
    }

    void OnSliderChanged(float value)
    {
        objectToRotate = GameObject.Find("TimeWalkObject");
        Debug.Log("object: " + objectToRotate);

        // How much we've changed
        float delta = value - this.previousValue;
        this.objectToRotate.transform.Rotate(Vector3.down * delta * 360);


        Debug.Log("object transform: " + objectToRotate.transform);

        // Set our previous value for the next change
        this.previousValue = value;
    }
}
