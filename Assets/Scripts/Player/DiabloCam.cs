using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabloCam : MonoBehaviour {

	public float scrollSpeed = 20f;

	public float topBounds = 0.97f, botBounds = 0.03f, leftBounds = 0.97f, rightBounds = 0.03f;

	public float minFov = 15f;
	public float maxFov = 90f;
	public float sensitivity = 30f;

	public Transform playerTransform;
	public float offsetX = -5f;
	public float offsetZ = -12;
	public float maximumDistance = 2f;
	public float camVelocity = 20f; //playerVelocity  (how fast you want the camera to follow the player)

	bool lockOnToPlayer = false;


	private float movementX;
	private float movementZ;


	// Update is called once per frame
	void Update () {


		if (!lockOnToPlayer) {
			if (Input.mousePosition.y <= Screen.height * topBounds) {
				transform.Translate (Vector3.back * Time.deltaTime * scrollSpeed, Space.World);
			}

			if (Input.mousePosition.y >= Screen.height * botBounds) {
				transform.Translate (Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);
			}

			if (Input.mousePosition.x <= Screen.width * rightBounds) {
				transform.Translate (Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
			}
			if (Input.mousePosition.x >= Screen.width * leftBounds) {
				transform.Translate (Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
			}
		} else {
			if (playerTransform != null) {

				movementX = ((playerTransform.position.x + offsetX - this.transform.position.x)) / maximumDistance;
				movementZ = ((playerTransform.position.z + offsetZ - this.transform.position.z)) / maximumDistance;
				transform.position += new Vector3((movementX * camVelocity * Time.deltaTime),0,(movementZ * camVelocity * Time.deltaTime));//playerTransform.position + offsetX;
			}
		}


		float fov = Camera.main.fieldOfView;
		fov -= Input.GetAxis ("Mouse ScrollWheel") * sensitivity;
		fov = Mathf.Clamp (fov, minFov, maxFov);
		Camera.main.fieldOfView = fov;

		if(Input.GetKeyDown(KeyCode.Y)){
			lockOnToPlayer = !lockOnToPlayer;
		}


	}


	public void setTarget(Transform target){
		playerTransform = target;
	}


}
