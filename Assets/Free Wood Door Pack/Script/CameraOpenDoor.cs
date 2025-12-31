using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraDoorScript
{
public class CameraOpenDoor : MonoBehaviour {
	public float DistanceOpen=1f;
	public GameObject text;
	Camera cam;
	//private int interactMask;
	// Use this for initialization
	void Start () {
		//interactMask = LayerMask.GetMask("XR Simulation");
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Debug.DrawRay(cam.transform.position,
              cam.transform.forward * DistanceOpen,
              Color.red);
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, DistanceOpen)) {
			if (hit.transform.GetComponent<DoorScript.Door>()) {
				text.SetActive(true);
				if (Input.GetKeyDown(KeyCode.R)) 
					hit.transform.GetComponent<DoorScript.Door>().OpenDoor();
			} else {
				text.SetActive(false);
			}
		} else {
			text.SetActive(false);
		} 
	}
}
}
