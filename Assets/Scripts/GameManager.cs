using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IVolumeControl
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    // Evento para notificar cambios (opcional)
    public event Action<int> AlCambiarDinero;

    [SerializeField] private float MasterVolume = 0.3f;
    [SerializeField] private int dinero = 0;

    private void Awake()
    {
        Debug.Log("GM Awake");
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", MasterVolume);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            
            // Aplico el volumen global (AudioListener)
            ApplyVolume();

            AlCambiarDinero?.Invoke(dinero);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public int Dinero
    {
        get => dinero;
        private set
        {
            dinero = value;
            AlCambiarDinero?.Invoke(dinero);
        }
    }

    /// <summary>
    /// A�ade la cantidad indicada al saldo y dispara el evento.
    /// </summary>
    public void generarDinero(int cantidad)
    {
        if (cantidad <= 0) return;
        Dinero += cantidad;
        NotificationManager.Instance.ShowMessage($"Dinero sumado: +{cantidad}, Total = {Dinero}");
    }

    /// <summary>
    /// Quita la cantidad indicada del saldo (por si acaso).
    /// </summary>
    public bool GastarDinero(int cantidad)
    {
        if (cantidad <= 0 || cantidad > Dinero) return false;
        Dinero -= cantidad;
        return true;
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
    // public void SetVolume(float newVolume)
    // {
    //     PlayerPrefs.SetFloat("MasterVolume", newVolume);
    //     ApplyVolume();
    // }
    public void SetVolume(float newVolume)
    {
        // 1) Actualizo el campo
        MasterVolume = Mathf.Clamp01(newVolume);
        // 2) Guardo en prefs
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.Save();
        // 3) Aplico globalmente
        ApplyVolume();
        // 4) Notifico al SoundController si existe
        if (SoundController.Instance != null)
            SoundController.Instance.UpdateVolume(MasterVolume);
    }

    public float GetVolume()
    {
        return MasterVolume;
    }

    private void ApplyVolume()
    {
        
        Debug.Log("Volumen inicial: " + MasterVolume);
        
        AudioListener.volume = MasterVolume;
    }
    /*FIN SECCION SONIDO*/
}
