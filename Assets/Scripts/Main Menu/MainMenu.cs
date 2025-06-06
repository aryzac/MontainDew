using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    SoundController soundController;

    //BUTTONS
    [SerializeField] private Button BtnPlay;
    [SerializeField] private Button BtnOptions;
    [SerializeField] private Button BtnReturn;
    [SerializeField] private Button BtnQuit;

    //SCREENS
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject optionScreen;

    //SOUNDS
    [SerializeField] private AudioClip Music;
    [SerializeField] private AudioClip ClickSound;

    //SLIDER
    [SerializeField] private Slider MasterVolumeSlider;

    //NOMBRE ESCENA A CARGAR
    [SerializeField] private string FirstScene;

    private IVolumeControl volumeControl;

    private void Start()
    {
        MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
        soundController = GetComponent<SoundController>();

        volumeControl = GameManager.Instance;
        MasterVolumeSlider.value = volumeControl.GetVolume();

        soundController.PlaySoundLoop(Music);
    }

    private void Awake()
    {
        BtnPlay.onClick.AddListener(Play);
        BtnOptions.onClick.AddListener(GotoOptions);
        BtnReturn.onClick.AddListener(GotoMenu);
        BtnQuit.onClick.AddListener(QuitGame);

        MasterVolumeSlider.onValueChanged.AddListener(SetVolumen);

        mainScreen.SetActive(true);
        optionScreen.SetActive(false);
    }

    private void Play()
    {
        soundController.PlaySound(ClickSound);
        GameManager.Instance.LoadLevel(FirstScene);
    }

    private void GotoOptions()
    {
        soundController.PlaySound(ClickSound);
        mainScreen.SetActive(false);
        optionScreen.SetActive(true);
    }

    private void GotoMenu()
    {
        soundController.PlaySound(ClickSound);
        mainScreen.SetActive(true);
        optionScreen.SetActive(false);
    }

    private void QuitGame()
    {
        soundController.PlaySound(ClickSound);
        Application.Quit();

        // Si estamos en el editor, detener la ejecución
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SetVolumen(float NewVolume)
    {
        PlayerPrefs.SetFloat("MasterVolume", NewVolume);
    }
}