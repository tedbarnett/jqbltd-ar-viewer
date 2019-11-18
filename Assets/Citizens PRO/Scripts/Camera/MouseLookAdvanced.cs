using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLookAdvanced : MonoBehaviour
{
	public float sensitivityX = 5F;
	public float sensitivityY = 5F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -90F;
	public float maximumY = 90F;
	
	public float smoothSpeed = 20F;

	float verticalAcceleration = 0f;
	
	float rotationX = 0F;
	float smoothRotationX = 0F;
	float rotationY = 0F;
	float smoothRotationY = 0F;
	Vector3 vMousePos;
	public float Speed = 100f;
	
	void Start()
	{
		rotationY = -transform.localEulerAngles.x;
		rotationX = transform.localEulerAngles.y;
		smoothRotationX = transform.localEulerAngles.y;
		smoothRotationY = -transform.localEulerAngles.x;
	}

	void Update()
	{
		verticalAcceleration = 0.0f;

		if (Input.GetMouseButtonDown (1)) 
		{
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

			Cursor.visible = !Cursor.visible;
		}

		if (Input.GetKey(KeyCode.Space)) { verticalAcceleration = 1.0f; }
		if (Input.GetKey(KeyCode.LeftShift)) { verticalAcceleration = -1.0f; }


        if(Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

		// smooth mouse look
		smoothRotationX += (rotationX - smoothRotationX) * smoothSpeed * Time.smoothDeltaTime;
		smoothRotationY += (rotationY - smoothRotationY) * smoothSpeed * Time.smoothDeltaTime;
		
		// transform camera to new direction
		transform.localEulerAngles = new Vector3(-smoothRotationY, smoothRotationX, 0);
		
		// handle camera movement via controller
		Vector3 inputMag = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector3 inputMoveDirection = transform.rotation * inputMag;
		transform.position += inputMoveDirection * Speed * Time.smoothDeltaTime;
		transform.position += new Vector3(0f, (Speed/2f) * verticalAcceleration * Time.smoothDeltaTime, 0);
		
		//transform.position += Vector3.up * (Input.GetAxis("VerticalOffset") * 10.0f * Time.smoothDeltaTime);
		transform.position += (transform.rotation * Vector3.forward) * Input.GetAxis("Mouse ScrollWheel") * 200.0f;
	}
}
