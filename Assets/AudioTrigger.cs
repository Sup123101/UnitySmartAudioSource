using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioLowPassFilter))]
[RequireComponent (typeof (AudioSource))]
/*
 Smart Audio Source: 
 Author: Richard (PT) Lai

 Professor: Jean-Luc Sinclair

Features:
	-Cone Attenuation
	-HPF Filter for far away sounds
	-Occlusion + Only raycasts when in bounds (Resource saving)
	-Fully scalable


Summary: 
  -Collider Trigger to pause/Play our Audio Source (Scales in Size) and Control if Raycasting is necessary
      TLDR- Saves resources
  -Using Player Coordinate + Audio Source Coordinates for check for angle -> LPF + Amp Mod for cone Attenuation
  -Using One Raycast to check for occlusion
  -Using the same Raycast grab the distance + tag for:
	-High Pass Filtering (for if you're far away)


Possible Improvements: 
 -Rather than Linear changes, Exponential changes could be fun to explore
 
For more Interesting works by me check out:
https://rlsawe.wixsite.com/rptlai
 */ 

public class AudioTrigger : MonoBehaviour {

	/*
    
    Public Variable Declaration area
    
    */
	// Audio Source we're controller
	public AudioSource sphereSound;

	//High Pass 
	public AudioHighPassFilter hpFilter;

	//Low Pass
	public AudioLowPassFilter lpFilter;

	//Used for RayCast Later
	private RaycastHit occluderRayHit;

	//Reference to Player
	GameObject listener;

	//Check to see if player or listener is within bounds of audible region
	public BoxCollider TriggerBox;

	//Max Amount of attenuation to LPF (Amount to cutoff from 20000hz by)
	public float lpCutOffAmount;

	//Max Amount of volume attenuation (Amount to lower from a max volume of 1)
	public float ampAtten;

	//max Distance that can be heard (from the audio source)
	public float maxAudioDistance; 

	//what is the max frequency we should move our hpf cutoff Frequency to
	public float hpCutOff;

	/*
    Local Variable Section
    */
	//Angle between front of Audio Source and Player
	float angle = 0;
	//If player is within bounds or not
	bool bounds = true;
	//If Sound is being Occluded already or not
	bool occluded = false;
	//Distance between Source and the Listener
	float dist;
	//distance amount that we get from 25% of max distance 
	float hpfdist;
	//using the above variable, calculate when does the highpassfilter stop
	float hpfstartdist;

	// Use this for initialization
	void Start () {
		//Find the player/listener
		listener = GameObject.Find("FirstPersonCharacter");
		lpFilter = GetComponent<AudioLowPassFilter> ();
		hpFilter = GetComponent<AudioHighPassFilter> ();
		sphereSound = GetComponent<AudioSource> ();

		/*
		 1. Make our Collider a trigger
		 2. Make it in same place as our source
		 3. Audio Sources are Circular thus 3D curve is radius. thus size should be 2 * Radius (Essentially the diameter)
		 */ 
		TriggerBox.isTrigger = true;
		TriggerBox.transform.position = transform.position;
		TriggerBox.size = new Vector3((maxAudioDistance * 2f), (maxAudioDistance * 2f), (maxAudioDistance * 2f));

		//Play the sound file, loop it, and specify volume. Change at will
		sphereSound.loop = true;

		//High Pass filter will kick in for the last 1/3rd distance away (as compared to max 3D Audio attenuation Curve)
		//Also Change .3f to whatever you wish.
		hpfdist = maxAudioDistance * .3f;
		hpfstartdist = maxAudioDistance - hpfdist;

	}

	//When it enters our trigger - Start Audio Source. -> Start the Raycasts
	void OnTriggerEnter (Collider other){
		bounds = true;
		sphereSound.Play ();
		//Debug.Log("start the raycast");

	}
	//When outside our Trigger - Stop Audio Source. -> Stop the Raycasts
	void OnTriggerExit(Collider other){
		sphereSound.Pause ();
		bounds = false;
		//Debug.Log("Stop the raycast");
	}

	void Update () {
		//only do anything if player is within bounds of audio source (as in can hear it)
		if (bounds == true) {
			
			// Calculate Distance Between Source and Listener
			dist = Vector3.Distance(listener.transform.position, transform.position);

			//Find angle between listener and source and also check if we need to occlude
			//if occluded, it is prioritized and and then we do the cone shape attenuation 
			//below function also does the occluding
			angleCalcOcc (listener);

			/*
			 essentially from between the max audible distance and the distance where we want high pass filter to end
			 we scale the current distance (between the 2 value points) as a value between 0 and 1 and then
			 multiple them to our HPF cutoff Frequency. Ex. at Max Audio Distance, HPF will be @ specified cutoff
			 and at End of HPF distance we have 0 as our HPF cut off frequency
			 */
			if (occluded == false) {
				if (dist >= hpfstartdist) {
					float hpfrelDist = dist - hpfstartdist;
					float hpfscale = hpfrelDist /(maxAudioDistance - hpfstartdist);
					float hpfFreq = hpfscale * hpCutOff;
					if (hpfFreq > hpCutOff) {
						hpFilter.cutoffFrequency = hpCutOff;
					} else {
						hpFilter.cutoffFrequency = hpfFreq;
					}
				}
				/*
				 The Cone Attenuation starts at 45 Degrees at either side (left being 315 degrees, right at 45 Degrees)
                 Should the Listener cross over these angles, start measuring the angle
				if the player is on the left side, scale 315~180 degress to become 0 ~ 180 degrees
				THUS on both sides of the sound source the angle difference will be 0~180 Degrees
				
				The Idea is @ 180 Degrees, we have max attenutation. Thus calculate the current angle difference from 
				above and divide that by 135 (as between 180 Degrees and 45 Degrees we have 135 Degrees in betwen).

				Take that value and minus it by 1. Why? @ 45 Degrees. 180-45 = 135. -> 135/135 = 1. ->1-1 = 0
											->CutoffAmount = 0 * max amount of cutoff = 0. (no cut off)
											Or in Reverse @ 180 Degrees. 180 - 180 = 0. -> 0/1 = 0. -> 1- 0 = 0.
											->Cutoff Amount = 1 * Max amount of cutoff = Cutoff Aount (Max Cut Off)

                Using the same Scaling Factor, Lower the Volume with max attenuation @ 180 Degrees and scale by
                parameters pertaining to volume.
				*/ 		


				if (angle < 315f && angle > 45f) {
					float cutoffF;
					float cAngle;
					if (angle > 180) {
						cAngle = 360 - angle;
					} else {
						cAngle = angle;
					}
					float difference = 180f - cAngle;
					float scalingF = difference / 135f; 
					cutoffF = 20000f - ((1f - scalingF) * lpCutOffAmount);
					lpFilter.cutoffFrequency = cutoffF;
					float volumeChange = 1f - ((1f - scalingF) * ampAtten);
					sphereSound.volume = volumeChange;

				} else {
					lpFilter.cutoffFrequency = 20000f;
					sphereSound.volume = 1f;
				}
			}
		}
	}
	//end update function



	void angleCalcOcc(GameObject obj)
	{
		
		//Know where the front is
		Vector3 frontDr = transform.forward;
		//Get Direction of where Source is relative to listener
		Vector3 raycastDir = obj.transform.position - transform.position; 
		//Shoot RayCast From Source to Listener
		if (Physics.Raycast (transform.position, raycastDir, out occluderRayHit, dist))
			{
			//Grab the Angle between Direction of Ray cast and the Front (with Fixed Y Axis)
			angle = Vector3.SignedAngle(raycastDir, frontDr, Vector3.up);
			//draw a green raycast for us to see
			Debug.DrawRay(transform.position, raycastDir, Color.green);
			//If Ray Cast hits a wall (or replace with != player) -- then Occlude
			if (occluderRayHit.collider.gameObject.tag == "Geometry")
			{

				//Debug.Log ("OCCLUDE!");
				lpFilter.cutoffFrequency =  3000f; 
				occluded = true;
			}


			//Debug.Log ("DO NOT OCCLUDE!");
			lpFilter.cutoffFrequency = 20000f; // otherwise no occlusion
			occluded = false;
			}
		//Make Negative Angles Scale from 0 -> 360
		if(angle < 0f) angle += 360f;
		print ("Angle is " + angle);
	}





	
}
