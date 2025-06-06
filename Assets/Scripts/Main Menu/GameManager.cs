using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IVolumeControl
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    [SerializeField] private float MasterVolume = 0.3f;

    private void Awake()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string sceneToLoad)
    {
        try
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /*SECCION SONIDO*/
    public void SetVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("MasterVolume", newVolume);
        ApplyVolume();
    }

    public float GetVolume()
    {
        return MasterVolume;
    }

    private void ApplyVolume()
    {
        AudioListener.volume = MasterVolume;
    }
    /*FIN SECCION SONIDO*/
}
