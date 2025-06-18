using UnityEngine;
using TMPro;

public class DineroUI : MonoBehaviour
{
    [Header("Arrastra aqu� el TextMeshProUGUI llamado DineroQuantity")]
    [SerializeField] private TextMeshProUGUI DineroQuantity;

    void Start()
    {
        // 1) Suscribirse al evento una vez Awake() de GameManager ya corri�
        if (GameManager.Instance != null)
            GameManager.Instance.AlCambiarDinero += ActualizarDinero;

        // 2) Mostrar inmediatamente el valor actual
        ActualizarDinero(GameManager.Instance != null
            ? GameManager.Instance.Dinero
            : 0);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AlCambiarDinero -= ActualizarDinero;
    }

    private void ActualizarDinero(int cantidad)
    {
        // Aqu� usamos la variable DineroQuantity directamente
        if (DineroQuantity != null)
            DineroQuantity.text = cantidad.ToString();
    }
}
