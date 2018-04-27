//using UnityEngine;
//using System.Collections;
//
//[RequireComponent (typeof (AudioSource))]
//
//public class Visualizer : MonoBehaviour
//{
//
//	public AudioClip theSound;
//
//	void Start()
//	{
//		audio.clip = theSound;
//
//		int numOfSamples = audio.clip.samples;   // 1647360
//		float[] samples = new float[numOfSamples];
//
//		audio.clip.GetData(samples, 0); //left channel
//		for (int i= 0; i < samples.Length; i++)
//		{
//			print (samples [i]);
//
//			// 0 UnityEngine.MonoBehaviour:print(Object) 887 times
//			// -6.103516E-05
//			// -0.0001220703
//			// 6.103516E-05 etc...
//
//		}
//	}
//}