using TMPro;
using UnityEngine;

public class BuyTeleport : MonoBehaviour
{
    [Header("Parametros del portal")]
    [SerializeField]int costoPortal;
    [SerializeField]string destino;
    [SerializeField]bool comprado = false;

    private Transporter transportador;
    private TextMeshProUGUI textoEstado;//Acá voy a mostrarle al flaco si tiene o no el tp y si no lo tiene cuanto le sale

    void Awake()
    {
        transportador = GetComponent<Transporter>();
        if (transportador == null)
            Debug.LogError("Imposible encontrar Transporter en este objeto");

        textoEstado = GetComponentInChildren<TextMeshProUGUI>();
        if (textoEstado == null)
            Debug.LogError("No se encontró el cuadro de texto a editar");
    }

    void Start()
    {
        //comprado significa si el portal viene por defecto adquirido o no
        if (comprado == false)
        {
            transportador.SetearPortal(false);
            textoEstado.text = "El portal a " + destino + $" Cuesta {costoPortal} de oro";
        }
        else
        {
            transportador.SetearPortal(true);
            textoEstado.text = "Portal adquirido";
        }
    }

    public void ComprarPortal()
    {
        if (GameManager.Instance.Dinero >= costoPortal)
        {
            transportador.SetearPortal(true);
            this.comprado = true;
            textoEstado.text = "Portal adquirido";
        }
        else
        {
            NotificationManager.Instance.ShowMessage("dinero insuficiente para comprar el portal");
        }
    }

    public bool PortalComprado()
    {
        return comprado;
    }
}
