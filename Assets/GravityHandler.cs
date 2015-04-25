﻿using UnityEngine;
using System.Collections;

public class GravityHandler : MonoBehaviour {

	Animator animateTheDude;
	bool characterRotating;
	float rotationRate =12f;
	public bool currentlyFlipped = false; //based on original oritenation flipped is flipped from beginning
	GameObject midpoint;

	// Use this for initialization
	void Start () {

		animateTheDude = GetComponent<Animator> ();
		characterRotating = false;
		midpoint = GameObject.Find ("MidpointBody");
		//roofHeight = GameObject.Find ("Roof").transform.position.y; //roof height
		//groundHeight = 0; 
		//pauseCanvas = GameObject.Find ("PauseCanvas");
		//pauseCanvas.SetActive (false);
		//isPaused = false;
		//pauseMenu = new Rect (0, 0, Screen.width, Screen.height);
		//box = GameObject.Find ("CubeBlockingDoor");
		//boxSource = box.GetComponent<AudioSource> ();
		//theGuy = GameObject.Find ("Dude");
	
	}
	
	// Update is called once per frame
	void Update () {
		bool onPlane = isOnPlane (); //could be up or down

		//Gravity controller
		if( !characterRotating && Input.GetKeyDown(KeyCode.G) && onPlane) //cannot rotate while in air, only near planes
		{
			animateTheDude.enabled = false; //turn off the animator during rotation
			animateTheDude.SetFloat("Direction",0); //stop rotation of guy because we are flipping, rotation is sensitive to the current rotation
			//set speed of rotation based on how far away wall is (if possible)
			characterRotating = true;
			Physics.gravity *= -1;
			currentlyFlipped = !currentlyFlipped; //flip this variable
		}
		
		rotate(); //BUGS are coming from the rotation being slightly off (like 0.00000000000000000000003 degrees off)
		
		renableAnimator (); //checks to see if animator can be reintroduced (based on if the character is rotation or not)

	}

	//need a function to smoothly rotate character. Should probably change speed of rotation depending on distance from walls.
	//Must ensure that character has flipped by the time he touches next wall
	//this might be useful http://codereview.stackexchange.com/questions/68217/rotating-a-character-upside-down-and-vice-versa
	private void rotateCharacter(){ 
		//var distance = Vector3.Distance(object1.transform.position, object2.transform.position);//calc distance between 2 objects
		Vector3 rot = this.transform.rotation.eulerAngles; 
		rot = new Vector3(rot.x,rot.y,rot.z+180);
		this.transform.rotation = Quaternion.Euler(rot);
	}
	/*
	private void rotate(){
		float totalRotation = 0;
		while (characterRotating) {
			if(totalRotation >= 180){
				characterRotating = false;
			}else{
				Vector3 rot = this.transform.rotation.eulerAngles;
				transform.Rotate(0, 0, rotationRate * Time.deltaTime);
				totalRotation += rotationRate * Time.deltaTime;
			}
		}

	}*/
	
	private void rotate(){
		if (characterRotating) {
			transform.Rotate (0, 0, rotationRate);
			float threshold = 5f;
			
			Vector3 rot = this.transform.rotation.eulerAngles; 
			//check if character is done rotating
			if (Mathf.Abs (rot.z) < threshold || Mathf.Abs(rot.z-360) < threshold) {//rightside up
				//smooth rotation
				rot = new Vector3 (rot.x, rot.y, 0);
				this.transform.rotation = Quaternion.Euler (rot);
				//signal rotation has ended
				characterRotating = false;
			} else if (Mathf.Abs (rot.z - 180) < threshold) {//upside down
				//smooth rotation
				rot = new Vector3 (rot.x, rot.y, 180);
				this.transform.rotation = Quaternion.Euler (rot);
				//signal rotation has ended
				characterRotating = false;
			}
		}
	}
	
	//this function checks to see if the character is rotating and if it is not, the animator can be reintroduced
	private void renableAnimator()
	{
		if (!characterRotating)
			animateTheDude.enabled = true;
		
	}

	public void respawn()
	{
		
		/*GameObject bcknd = GameObject.Find ("Main Camera");
		AudioSource audio = bcknd.GetComponent<AudioSource> ();
		DontDestroyOnLoad (audio);*/
		//reset everything
		if(currentlyFlipped)
			Physics.gravity*=-1; //flip
		currentlyFlipped = !currentlyFlipped; //reset flip bool
		
		Application.LoadLevel (Application.loadedLevelName); //reload level
	}

	//helper funciton to check and see if player is closeEnough to a plane (roof or floor)
	//in future iterations, this can include moving platforms and other areas that will be acceptable for rotation
	//true if close enough, false o.w
	private bool isOnPlane(){
		float distance = 1f;
		RaycastHit hit;

		if(currentlyFlipped){
			//params are origin, direction of ray, "to hit" distance of ray (baiscalyl how far you want it to check)
			Physics.Raycast (midpoint.transform.position, Vector3.up, out hit,distance);
			//go until it hits something within 1
			distance = hit.distance;
		}
		else{
			Physics.Raycast (midpoint.transform.position, -Vector3.up, out hit,distance);
			distance = hit.distance;
			
		}

		if (distance != 0) //if u hit anyhting thne it isn't 0 and you are "on a plane"
			return true;
		return false;

	}
}
