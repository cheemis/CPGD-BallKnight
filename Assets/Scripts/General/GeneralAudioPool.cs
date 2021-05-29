using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GeneralAudioPool : Singleton<GeneralAudioPool> {
	public AudioMixerGroup defaultAudioMixerGroup = null;
	public int initialAudioSourcePoolCount = 30;

	private List<AudioSource> availableAudioSources = new List<AudioSource>();
	private List<AudioSource> allAudioSources = new List<AudioSource>();
	void Awake() {
		if (transform.childCount <= 0) {
			GenerateAudioSourcePool();
		}
	}

	private void GenerateAudioSourcePool() {
		GameObject audioSourcesHolder = new GameObject("audio_sources");
		audioSourcesHolder.transform.parent = transform;

		for (int i = 0; i < initialAudioSourcePoolCount; i++) {
			GameObject audioSourceObject = new GameObject($"audio_source_{i}");
			audioSourceObject.transform.parent = audioSourcesHolder.transform;

			AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = defaultAudioMixerGroup;
			audioSource.playOnAwake = false;

			availableAudioSources.Add(audioSource);
			allAudioSources.Add(audioSource);
		}
	}

	private void AddNewAudioSourceToPool() {
		GameObject stepAudioSourceObject = new GameObject($"step_audio_source_{allAudioSources.Count}");
		stepAudioSourceObject.transform.parent = transform.GetChild(0);

		AudioSource audioSource = stepAudioSourceObject.AddComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = defaultAudioMixerGroup;
		audioSource.playOnAwake = false;

		availableAudioSources.Add(audioSource);
		allAudioSources.Add(audioSource);
	}

	public AudioSource PlaySound(AudioClip audioClip, float audioVolume, float audioPitch, AudioMixerGroup audioMixerGroup = null) {
		AudioSource selectedAudioSource = GenericPlaySound(audioClip, audioVolume, audioPitch, audioMixerGroup);

		StartCoroutine(WaitForAudioSourceToFinishCo(selectedAudioSource));

		return selectedAudioSource;
	}

	private AudioSource GenericPlaySound(AudioClip audioClip, float audioVolume, float audioPitch, AudioMixerGroup audioMixerGroup) {
		if (transform.childCount <= 0) {
			GenerateAudioSourcePool();
		}

		if (availableAudioSources.Count <= 0) {
			AddNewAudioSourceToPool();
		}

		AudioSource selectedAudioSource = availableAudioSources[0];

		selectedAudioSource.clip = audioClip;
		selectedAudioSource.volume = audioVolume;
		selectedAudioSource.pitch = audioPitch;

		// Use default if no override is provided
		if (audioMixerGroup != null) {
			selectedAudioSource.outputAudioMixerGroup = audioMixerGroup;
		}
		else {
			selectedAudioSource.outputAudioMixerGroup = defaultAudioMixerGroup;
		}

		selectedAudioSource.Play();

		availableAudioSources.RemoveAt(0);

		return selectedAudioSource;
	}

	public AudioSource PlayDistanceBasedSound(AudioClip audioClip, float maxAudioVolume,
		float audioPitch, Transform transform1, Transform transform2, float maxDistance = 20.0f, AudioMixerGroup audioMixerGroup = null) {

		float distance = Vector3.Distance(transform1.position, transform2.position);
		if (distance <= maxDistance) {
			float audioVolume = Mathf.Lerp(0.0f, maxAudioVolume, 1.0f - (distance / maxDistance));

			AudioSource selectedAudioSource = GenericPlaySound(audioClip, audioVolume, audioPitch, audioMixerGroup);

			StartCoroutine(PlayDistanceBasedSoundCo(selectedAudioSource, maxAudioVolume, transform1, transform2, maxDistance));

			return selectedAudioSource;
		}
		else {
			return null;
		}
	}

	private IEnumerator WaitForAudioSourceToFinishCo(AudioSource audioSource) {
		while (audioSource.isPlaying)
			yield return null;

		availableAudioSources.Add(audioSource);
	}

	private IEnumerator PlayDistanceBasedSoundCo(AudioSource audioSource, float maxAudioVolume,
		Transform transform1, Transform transform2, float maxDistance) {

		while (audioSource.isPlaying) {
			if (transform1 != null && transform2 != null) {
				float distance = Vector3.Distance(transform1.position, transform2.position);
				if (distance <= maxDistance) {
					float audioVolume = Mathf.Lerp(0.0f, maxAudioVolume, 1.0f - (distance / maxDistance));
					audioSource.volume = audioVolume;
				}
				else {
					audioSource.volume = 0.0f;
				}
			}
			yield return null;
		}

		availableAudioSources.Add(audioSource);
	}
}
