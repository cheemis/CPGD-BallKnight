using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour {
    public AudioMixer mainAudioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public CanvasGroup defaultMenuGroup;
    public CanvasGroup optionsMenuGroup;

    private CanvasGroup mainPauseMenuGroup;

    private MainPlayerController mainPlayerController;
    private GameObject mouseCursorObject;

    private bool isPaused = false;
    void Awake() {
        mainPauseMenuGroup = GetComponent<CanvasGroup>();

        mainPlayerController = FindObjectOfType<MainPlayerController>();
        mouseCursorObject = FindObjectOfType<MouseCursorHandler>().gameObject;

        SetMasterVolume(0.5f);

        UnpauseGame();
    }

    void Update() {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Backslash)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (!(!isPaused && !mainPlayerController.CheckIfInputEnabled())) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnPauseButtonDown();
            }
        }
    }

    private void EnableCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    private void DisableCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void UnpauseGame() {
        DisableCanvasGroup(mainPauseMenuGroup);
        EnableCanvasGroup(defaultMenuGroup);
        DisableCanvasGroup(optionsMenuGroup);

        Cursor.visible = false;
        mouseCursorObject.SetActive(true);
        mainPlayerController.EnableInput();
        Time.timeScale = 1.0f;

        // Return (or set newly changed) sfx volume on unpause
        mainAudioMixer.SetFloat("sfxVolume", GameBrain.Instance.sfxVolume);

        isPaused = false;
    }

    private void PauseGame() {
        EnableCanvasGroup(mainPauseMenuGroup);
        EnableCanvasGroup(defaultMenuGroup);
        DisableCanvasGroup(optionsMenuGroup);

        Cursor.visible = true;
        mouseCursorObject.SetActive(false);
        mainPlayerController.DisableInput();
        Time.timeScale = 0.0f;

        // Mute sound effects while paused
        mainAudioMixer.SetFloat("sfxVolume", -80.0f);

        isPaused = true;
    }


    public void OnPauseButtonDown() {
        // If unpausing
        if (isPaused) {
            UnpauseGame();
        }
        // If pausing
        else {
            PauseGame();
        }
    }

    public void OnOpenOptionsButtonDown() {
        DisableCanvasGroup(defaultMenuGroup);
        EnableCanvasGroup(optionsMenuGroup);
    }

    public void OnOptionsReturnButtonDown() {
        EnableCanvasGroup(defaultMenuGroup);
        DisableCanvasGroup(optionsMenuGroup);
    }

    public void OnQuitButtonDown() {
        return;

        // Make sure player can't use input while scene is loading
        mainPlayerController.DisableInput();
        mainPauseMenuGroup.interactable = false;
        mainPauseMenuGroup.blocksRaycasts = false;

        SceneManager.LoadScene("MainMenu");
	}

    public bool CheckIfPaused() {
        return isPaused;
	}

    public void SetMasterVolume(float sliderValue) {
        GameBrain.Instance.masterVolume = Mathf.Log10(sliderValue) * 20.0f;
        mainAudioMixer.SetFloat("masterVolume", GameBrain.Instance.masterVolume);
	}

    public void SetMusicVolume(float sliderValue) {
        GameBrain.Instance.musicVolume = Mathf.Log10(sliderValue) * 20.0f;
        mainAudioMixer.SetFloat("musicVolume", GameBrain.Instance.musicVolume);
    }

    public void SetSFXVolume(float sliderValue) {
        GameBrain.Instance.sfxVolume = Mathf.Log10(sliderValue) * 20.0f;
    }
}
