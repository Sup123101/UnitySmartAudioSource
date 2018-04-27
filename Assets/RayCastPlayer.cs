using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioLowPassFilter))]
[RequireComponent (typeof (AudioHighPassFilter))]
[RequireComponent (typeof (AudioSource))]

public class RayCastPlayer : MonoBehaviour {

	//public Vector3 PlayerPos;
	//public GameObject playerObject;

	public float currentVol;
	public float currentPitch;
	public AudioSource source;
	public AudioLowPassFilter lpFilter;
	public AudioHighPassFilter hpFilter;

	private RaycastHit occluderRayHit;
	//private GameObject listenerObj;
	  GameObject listener;


	// Use this for initialization
	void Start () {


		listener = GameObject.Find("Player");

		source = gameObject.GetComponent<AudioSource>();
		lpFilter = gameObject.GetComponent<AudioLowPassFilter>();
		hpFilter = gameObject.GetComponent<AudioHighPassFilter>();
		source.playOnAwake = true;


	}
	
	// Update is called once per frame
	void Update () {


		lpFilter.cutoffFrequency = GetOcclusionFreq (listener, 10);

		Debug.Log ("Scanning");


		}



	private float GetOcclusionFreq (GameObject obj, float distance){

		Debug.Log ("GetOcclusion");


		Vector3 raycastDir = obj.transform.position - transform.position; 
		Debug.DrawRay (transform.position, raycastDir, Color.red); 

		if (Physics.Raycast (transform.position, raycastDir, out occluderRayHit, distance)) // raycast to listener object
		{
			if (occluderRayHit.collider.gameObject != listener) // occlude if raycast does not hit listener object
			{

				Debug.Log ("OCCLUDE!");
				return 3000f; 

			}
				
		}

		Debug.Log ("DO NOT OCCLUDE!");
		return 20000f; // otherwise no occlusion

	}



}


		


