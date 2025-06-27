using System.Collections;
using UnityEngine;
using TMPro;

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
        // opcional: DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Muestra un mensaje durante 'duration' segundos. 
    /// Si no le pasas duration (o es ≤ 0), usa defaultDuration.
    /// </summary>
    public void ShowMessage(string msg, float duration = -1f)
    {
        StopAllCoroutines();
        notificationText.text = msg;
        float dur = (duration > 0f) ? duration : defaultDuration;
        StartCoroutine(ClearAfter(dur));
    }

    private IEnumerator ClearAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        notificationText.text = "";
    }
}
