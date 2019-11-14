using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour {
    private Animator gull;
    public GameObject MainCamera;

    void Start ()
    {
        gull = GetComponent<Animator>();
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gull.SetBool("walk", true);
            gull.SetBool("idle", false);
            gull.SetBool("landing", false);
        }
        if ((Input.GetKeyUp(KeyCode.W))||(Input.GetKeyUp(KeyCode.E))||(Input.GetKeyUp(KeyCode.A))||(Input.GetKeyUp(KeyCode.D)))
        {
            gull.SetBool("idle", true);
            gull.SetBool("walk", false);
            gull.SetBool("eat", false);
            gull.SetBool("walkleft", false);
            gull.SetBool("walkright", false);
            gull.SetBool("fly", true);
            gull.SetBool("flyleft", false);
            gull.SetBool("flyright", false);
        }
        if ((gull.GetCurrentAnimatorStateInfo(0).IsName("landing")) || (gull.GetCurrentAnimatorStateInfo(0).IsName("idle")) || (gull.GetCurrentAnimatorStateInfo(0).IsName("takeoff")))
        {
            gull.SetBool("landing", false);
            gull.SetBool("takeoff", false);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            gull.SetBool("takeoff", true);
            gull.SetBool("idle", false);
            gull.SetBool("landing", false);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if ((gull.GetCurrentAnimatorStateInfo(0).IsName("fly")))
            {
                Debug.Log("fly is current");
                gull.SetBool("fly", false);
                gull.SetBool("landing", true);
                gull.SetBool("takeoff", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            gull.SetBool("glide", true);
            gull.SetBool("fly", false);
            gull.SetBool("takeoff", false);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            gull.SetBool("fly", true);
            gull.SetBool("glide", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gull.SetBool("idle", false);
            gull.SetBool("eat", true);
            gull.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gull.SetBool("walkleft", true);
            gull.SetBool("walkright", false);
            gull.SetBool("walk", false);
            gull.SetBool("idle", false);
            gull.SetBool("flyleft", true);
            gull.SetBool("flyright", false);
            gull.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gull.SetBool("walkright", true);
            gull.SetBool("walkleft", false);
            gull.SetBool("walk", false);
            gull.SetBool("idle", false);
            gull.SetBool("flyright", true);
            gull.SetBool("flyleft", false);
            gull.SetBool("fly", false);
        }
        if (Input.GetKeyDown("down"))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = false;
        }
        if (Input.GetKeyUp("down"))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = true;
        }
    }
}
