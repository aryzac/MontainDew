using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NotificationManager : MonoBehaviour
{

    public static NotificationManager Instance { get; private set; }

    [Header("Referencia al TextMeshPro en tu HUD")]
    public TextMeshProUGUI notificationText;

    [Header("Duración por defecto (segundos)")]
    public float defaultDuration = 2f;

    void Awake()
    {
        // Implementación básica de singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    /// <summary>
    /// Al cargar la escena, bueno el elemento que va a mostrar las notificaciones
    /// Esto lo tengo que hacer porque como cambie de lugar el gamemanager porque se rompia todo, ahora translado las notificaciones desde el MainMenu
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Busca en la nueva escena el GameObject de tu texto (ajusta el nombre)
        var go = GameObject.Find("Notificaciones");
        if (go != null)
        {
            notificationText = go.GetComponent<TextMeshProUGUI>();
            if (notificationText == null)
                Debug.LogError("NotificationManager: 'Notificaciones' no tiene TextMeshProUGUI");
        }
        else
        {
            Debug.LogWarning($"NotificationManager: no encontró GameObject 'Notificaciones' en la escena {scene.name}");
        }
    }

    /// <summary>
    /// Muestra un mensaje durante 'duration' segundos. 
    /// Si no le pasas duration (o es ≤ 0), usa defaultDuration.
    /// </summary>
    public void ShowMessage(string msg, float duration = -1f)
    {
        try
        {
            StopAllCoroutines();
            notificationText.text = msg;
            float dur = (duration > 0f) ? duration : defaultDuration;
            StartCoroutine(ClearAfter(dur));
        }
        catch {
        }
    }

    private IEnumerator ClearAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        notificationText.text = "";
    }
}
