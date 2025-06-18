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
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            AlCambiarDinero?.Invoke(dinero);
        }
        else
        {
            Destroy(gameObject);
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
    /// Añade la cantidad indicada al saldo y dispara el evento.
    /// </summary>
    public void AñadirDinero(int cantidad)
    {
        if (cantidad <= 0) return;
        Dinero += cantidad;
        Debug.Log($"[ControladorJuego] Dinero sumado: +{cantidad}, Total = {Dinero}");
    }

    /// <summary>
    /// Quita la cantidad indicada del saldo (por si acaso).
    /// </summary>
    public bool GastarDinero(int cantidad)
    {
        if (cantidad <= 0 || cantidad > Dinero) return false;
        Dinero -= cantidad;
        Debug.Log($"[ControladorJuego] Dinero gastado: -{cantidad}, Total = {Dinero}");
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
