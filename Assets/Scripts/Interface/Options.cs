using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Options : MonoBehaviour
{
    // Graphics
    public CustomDropdown displayResolution;
    public Canvas canvas;
    public GameObject displayResolutionContainer;
    public Toggle fullscreenToggle;
    public UnityEvent onDisplaySettingsChange;

    // Audio
    //public Slider masterVolume;
    //public Slider soundEffects;

    // Other
    //public Text buildRevisionText;

    protected void Awake()
    {
        displayResolution.ClearOptions();
    }

    protected void Start()
    {
        List<string> resNames = new List<string>();

        foreach (Vector2 res in GameSettings.Instance.Resolutions)
        {
            // format 16:10 - 1680 x 1050
            string name = "" + res.x + " x " + res.y;
            resNames.Add(name);
        }
        displayResolution.AddOptions(resNames);

        fullscreenToggle.isOn = GameSettings.Instance.Fullscreen;
        displayResolutionContainer.SetActive(GameSettings.Instance.Fullscreen);
        displayResolution.SetValueWithoutNotify(GameSettings.Instance.Resolutions.FindIndex(res => res.x == Screen.currentResolution.width && res.y == Screen.currentResolution.height));

        if (Screen.currentResolution.width < 1600)
        {
            canvas.scaleFactor = 0.5f;
            onDisplaySettingsChange.Invoke();
        }


        //SetBuildInformation();

        //Callbacks
        displayResolution.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChange);
        //masterVolume.onValueChanged.AddListener((value) => { GameSettings.Instance.SetMasterVolume(value); playSoundEffect(AudioMain.VOLUME_TEST); });
        //soundEffects.onValueChanged.AddListener((value) => { GameSettings.Instance.SetSFXVolume(value); playSoundEffect(AudioMain.VOLUME_TEST); });
    }

    private void OnFullscreenChange(bool a_fullscreen)
    {
        GameSettings.Instance.SetFullscreen(a_fullscreen);
        displayResolutionContainer.SetActive(GameSettings.Instance.Fullscreen);
        GameSettings.Instance.SetResolution(GameSettings.Instance.DisplayResolution, canvas, false);
    }

    //private void SetBuildInformation()
    //{
    //    if (!ApplicationBuildIdentifier.Instance.GetHasInformation())
    //        ApplicationBuildIdentifier.Instance.GetManifest();
    //
    //    buildDateText.text = ApplicationBuildIdentifier.Instance.GetBuildTime();
    //    buildRevisionText.text = ApplicationBuildIdentifier.Instance.GetGitTag();
    //}

    private void OnResolutionChanged(int resolutionIndex)
    {
        GameSettings.Instance.SetResolution(resolutionIndex, canvas);
        onDisplaySettingsChange.Invoke();
    }

    //private void playSoundEffect(string audioID)
    //{
    //    AudioSource audioSource = AudioMain.Instance.GetAudioSource(audioID);
    //    if (audioSource != null && !audioSource.isPlaying)
    //    {
    //        audioSource.Play();
    //    }
    //}
}