using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions {
	static Dictionary<AudioSource, Coroutine> fadeInCoByAudioSource = new Dictionary<AudioSource, Coroutine>();
	static Dictionary<AudioSource, bool> fadeInActiveStateByAudioSource = new Dictionary<AudioSource, bool>();

	static Dictionary<AudioSource, Coroutine> fadeOutCoByAudioSource = new Dictionary<AudioSource, Coroutine>();
	static Dictionary<AudioSource, bool> fadeOutActiveStateByAudioSource = new Dictionary<AudioSource, bool>();

	public static void FadeIn (this AudioSource audioSource, float fadeDuration = 0.25f, float initialVolume = 0.05f) {
		FadeInTo(audioSource, audioSource.volume, fadeDuration, initialVolume);
	}

	public static void FadeInTo(this AudioSource audioSource, float endVolume, float fadeDuration = 0.25f, float initialVolume = 0.05f) {
		FadeInLauncher(audioSource, endVolume, fadeDuration, initialVolume, false);
	}

	public static void FadeInNoStart(this AudioSource audioSource, float endVolume, float fadeDuration = 0.25f, float initialVolume = 0.05f) {
		FadeInLauncher(audioSource, endVolume, fadeDuration, initialVolume, true);
	}

	private static void FadeInLauncher(this AudioSource audioSource, float endVolume, float fadeDuration, float initialVolume, bool noStart) {
		// End previous fade in coroutine if already active
		StopFadeInIfNeeded(audioSource, false);

		// End fade out coroutine if active
		StopFadeOutIfNeeded(audioSource, true);

		fadeInActiveStateByAudioSource[audioSource] = true;
		fadeInCoByAudioSource[audioSource] = RoutineRunner.Instance.StartCoroutine(FadeInCo(audioSource, initialVolume, endVolume, fadeDuration, noStart));
	}


	private static IEnumerator FadeInCo (AudioSource audioSource, float initialVolume, float endVolume, float fadeDuration, bool noStart) {
		float timeElapsed = 0.0f;

		audioSource.volume = initialVolume;

		if (!noStart) {
			audioSource.Play();
		}

		while (timeElapsed < fadeDuration) {
			if (!audioSource) {
				fadeInCoByAudioSource.Clear();
				fadeOutCoByAudioSource.Clear();
				fadeInActiveStateByAudioSource.Clear();
				fadeOutActiveStateByAudioSource.Clear();
				yield break;
			}
			audioSource.volume = Mathf.Lerp(initialVolume, endVolume, timeElapsed / fadeDuration);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		audioSource.volume = endVolume;

		fadeInActiveStateByAudioSource[audioSource] = false;
	}

	public static void FadeOut(this AudioSource audioSource, float fadeDuration = 0.25f) {
		FadeOutTo(audioSource, 0.0f, fadeDuration);
	}

	public static void FadeOutTo(this AudioSource audioSource, float endVolume, float fadeDuration = 0.25f) {
		FadeOutLauncher(audioSource, endVolume, fadeDuration, false);
	}

	public static void FadeOutNoStop(this AudioSource audioSource, float endVolume, float fadeDuration = 0.25f) {
		FadeOutLauncher(audioSource, endVolume, fadeDuration, true);
	}

	private static void FadeOutLauncher (this AudioSource audioSource, float endVolume, float fadeDuration, bool noStop) {
		// End fade out coroutine if active
		StopFadeOutIfNeeded(audioSource, false);

		// End previous fade in coroutine if already active
		StopFadeInIfNeeded(audioSource, true);

		fadeOutActiveStateByAudioSource[audioSource] = true;
		fadeOutCoByAudioSource[audioSource] = RoutineRunner.Instance.StartCoroutine(FadeOutCo(audioSource, endVolume, fadeDuration, noStop));
	}

	private static IEnumerator FadeOutCo(AudioSource audioSource, float endVolume, float fadeDuration, bool noStop) {
		float timeElapsed = 0.0f;

		float initialVolume = audioSource.volume;

		while (timeElapsed < fadeDuration) {
			if (!audioSource) {
				fadeInCoByAudioSource.Clear();
				fadeOutCoByAudioSource.Clear();
				fadeInActiveStateByAudioSource.Clear();
				fadeOutActiveStateByAudioSource.Clear();
				yield break;
			}
			audioSource.volume = Mathf.Lerp(initialVolume, endVolume, timeElapsed / fadeDuration);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		audioSource.volume = endVolume;

		if (!noStop) {
			audioSource.Stop();
			audioSource.volume = initialVolume;
		}

		fadeOutActiveStateByAudioSource[audioSource] = false;
	}

	public static bool FadeInIsActive(this AudioSource audioSource) {
		return fadeInActiveStateByAudioSource.ContainsKey(audioSource) && fadeInActiveStateByAudioSource[audioSource];
	}

	public static bool FadeOutIsActive(this AudioSource audioSource) {
		return fadeOutActiveStateByAudioSource.ContainsKey(audioSource) && fadeOutActiveStateByAudioSource[audioSource];
	}

	public static void StopFadeIn(this AudioSource audioSource) {
		StopFadeInIfNeeded(audioSource, true);
	}

	public static void StopFadeOut(this AudioSource audioSource) {
		StopFadeOutIfNeeded(audioSource, true);
	}

	private static void StopFadeInIfNeeded(AudioSource audioSource, bool changeActiveState) {
		if (fadeInActiveStateByAudioSource.ContainsKey(audioSource) && fadeInActiveStateByAudioSource[audioSource]) {
			if (fadeInCoByAudioSource.ContainsKey(audioSource)) {
				RoutineRunner.Instance.StopCoroutine(fadeInCoByAudioSource[audioSource]);

				if (changeActiveState)
					fadeInActiveStateByAudioSource[audioSource] = false;
			}
			else {
				Debug.LogError("Audio Source found active state for fade without a corresponding coroutine");
			}
		}
	}

	private static void StopFadeOutIfNeeded(AudioSource audioSource, bool changeActiveState) {
		if (fadeOutActiveStateByAudioSource.ContainsKey(audioSource) && fadeOutActiveStateByAudioSource[audioSource]) {
			if (fadeOutCoByAudioSource.ContainsKey(audioSource)) {
				RoutineRunner.Instance.StopCoroutine(fadeOutCoByAudioSource[audioSource]);

				if (changeActiveState)
					fadeOutActiveStateByAudioSource[audioSource] = false;
			}
			else {
				Debug.LogError("Audio Source found active state for fade without a corresponding coroutine");
			}
		}
	}
}
