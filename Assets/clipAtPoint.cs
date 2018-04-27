using UnityEngine;
using System.Collections;

public class ClipAtPoint
{
	//ClipAtPointPlus by Chase Harris
	//Version 0.0.1

	//Useage:
	//This was made to solve some of the issues encountered with PlayClipAtPoint, namely that it couldn't be used with the built-in Unity audio mixers.
	//
	//For example useage, see comments at end
	//
	//All the parameters from AudioSource that would realistically be needed are present.
	//Notice that things like "loop" are missing - this is a safety measure. You don't want to loop an AudioSource you don't have further access to. 
	//If you want it to loop, put it in a coroutine that will .Play() every (AudioClip.length)
	private bool bypassEffects = false;
	public bool BypassEffects
	{
		get { return bypassEffects; }
		set { bypassEffects = value; }
	}

	private bool bypassListenerEffects = false;
	public bool BypassListenerEffects
	{
		get { return bypassListenerEffects; }
		set { bypassListenerEffects = value; }
	}

	private bool bypassReverbZones = false;
	public bool BypassReverbZones
	{
		get { return bypassReverbZones; }
		set { bypassReverbZones = value; }
	}

	private float dopplerLevel = 1.0f;
	public float DopplerLevel
	{
		get { return dopplerLevel; }
		set { dopplerLevel = value; }
	}

	private bool ignoreListenerPause = false;
	public bool IgnoreListenerPause
	{
		get { return ignoreListenerPause; }
		set { ignoreListenerPause = value; }
	}

	private bool ignoreListenerVolume = false;
	public bool IgnoreListenerVolume
	{
		get { return ignoreListenerVolume; }
		set { ignoreListenerVolume = value; }
	}

	private float maxDistance = 500f;
	public float MaxDistance
	{
		get { return maxDistance; }
		set { maxDistance = value; }
	}

	private float minDistance = 1.0f;
	public float MinDistance
	{
		get { return minDistance; }
		set { minDistance = value; }
	}

	private bool mute = false;
	public bool Mute
	{
		get { return mute; }
		set { mute = value; }
	}

	private UnityEngine.Audio.AudioMixerGroup outputAudioMixerGroup;
	public UnityEngine.Audio.AudioMixerGroup OutputAudioMixerGroup
	{
		get { return outputAudioMixerGroup; }
		set { outputAudioMixerGroup = value; }
	}

	private float panStereo = 0.0f;
	public float PanStereo
	{
		get { return panStereo; }
		set { panStereo = value; }
	}

	private float pitch = 1.0f;
	public float Pitch
	{
		get { return pitch; }
		set { pitch = value; }
	}

	private int priority = 128;
	public int Priority
	{
		get { return priority; }
		set { priority = value; }
	}

	private float reverbZoneMix = 1.0f;
	public float ReverbZoneMix
	{
		get { return reverbZoneMix; }
		set { reverbZoneMix = value; }
	}

	private AudioRolloffMode rolloffmode = AudioRolloffMode.Logarithmic;
	public AudioRolloffMode Rolloffmode
	{
		get { return rolloffmode; }
		set { rolloffmode = value; }
	}

	private float spatialBlend = 0f;
	public float SpatialBlend
	{
		get { return spatialBlend; }
		set { spatialBlend = value; }
	}

	private float spread = 0f;
	public float Spread
	{
		get { return spread; }
		set { spread = value; }
	}

	private float volume = 1f;
	public float Volume
	{
		get { return volume; }
		set { volume = value; }
	}

	//Call this to play the clip at the point!
	public void Play(AudioClip clip, Vector3 playPosition)
	{
		GameObject soundObject = new GameObject ("tempSound");
		soundObject.transform.position = playPosition;
		AudioSource audioSource = soundObject.AddComponent<AudioSource> ();

		audioSource.bypassEffects = bypassEffects;
		audioSource.bypassListenerEffects = bypassListenerEffects;
		audioSource.bypassReverbZones = bypassReverbZones;
		audioSource.clip = clip;
		audioSource.dopplerLevel = dopplerLevel;
		audioSource.ignoreListenerPause = ignoreListenerPause;
		audioSource.ignoreListenerVolume = ignoreListenerVolume;
		audioSource.maxDistance = maxDistance;
		audioSource.minDistance = minDistance;
		audioSource.outputAudioMixerGroup = outputAudioMixerGroup;
		audioSource.panStereo = panStereo;
		audioSource.pitch = pitch;
		audioSource.priority = priority;
		audioSource.reverbZoneMix = reverbZoneMix;
		audioSource.rolloffMode = rolloffmode;
		audioSource.spatialBlend = spatialBlend;
		audioSource.spread = spread;
		audioSource.volume = volume;

		audioSource.Play ();
		MonoBehaviour.Destroy (soundObject, clip.length + 0.01f);
	}
	//
	//Example (same as PlayClipAtPoint, but assigns to a mixer group called "ExampleGroup":
	//
	//private ClipAtPointPlus source;
	//
	//void Start()
	//{
	//    source = new ClipAtPointPlus();
	//}
	//
	//void SomeFunction ()
	//{
	//    source.OutputAudioMixerGroup = mixer.FindMatchingGroups("ExampleGroup")[0];
	//    source.Play (exampleClip, exampleTransform.position);
	//{
	//
}
