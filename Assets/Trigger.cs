using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {


	[SerializeField]
	AudioClip[] seaGull;
	[SerializeField]
	int currentClip;
	[SerializeField]
	int preClip;
	[SerializeField]
	Vector3 position;

		void Start(){
			seaGull = Resources.LoadAll<AudioClip>("Sounds/Object Sounds/Seagull");
		}
	


	void OnTriggerEnter(Collider target){

		//Unity checks to see whether the object is a player (or an object tagged player anyhow")


		//If the triggered variable has not been set to true the sound will start

		print ("Triggered");
		StartCoroutine ("Play");

	}

	float randomise(float max){//set posX posZ to a random number
		while(true){
			float value = Random.Range (5f,max);
			float check = Random.Range (5f,max);
			if (value > check) {
				if (Random.value <= 0.5f)
					value *= -1f;
				return value;
			}
		}
	}

	Vector3 seagullPos(){
		
			position = new Vector3(
				transform.position.x + randomise(15f), 
				transform.position.y + randomise(10f),
				transform.position.z + randomise(15f));

		return position;
	}


	IEnumerator Play(){


		print ("InCORoutine");
			if(currentClip == preClip)
				currentClip = (int)Random.Range (0f,seaGull.Length);

			GameObject soundObject = new GameObject("tempsound");
			//soundObject.transform.position = seagullPos ();

			soundObject.transform.position = seagullPos ();
			AudioSource audiosource = soundObject.AddComponent<AudioSource> ();
			audiosource.clip = seaGull [currentClip];
			audiosource.rolloffMode = AudioRolloffMode.Logarithmic;
			//audiosource.outputAudioMixerGroup = seagullmixer;
			audiosource.dopplerLevel = 0f;
			audiosource.spatialBlend = 1.0f;
			audiosource.maxDistance = 50f;
			audiosource.Play();
			preClip = currentClip;
			yield return new WaitForSeconds (Random.Range(seaGull[currentClip].length+5f,seaGull[currentClip].length+10f));
			Destroy (soundObject);
		}
	


	}


	









