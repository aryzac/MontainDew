using UnityEngine;

public class Transporter : MonoBehaviour
{
    [Header("Destino del portal")]
    public Transform destino;

    [Header("Rotación al llegar (en grados, eje Y)")]
    public float rotacionYAlLlegar = 90f;

    private bool puedeTeletransportar = true;
    private BuyTeleport EstadoPortal;

    public void SetearPortal(bool Estado)
    {
        puedeTeletransportar = Estado;
    }

    private void Awake()
    {
        EstadoPortal = GetComponent<BuyTeleport>();
        if (EstadoPortal == null)
           // Debug.Log("Imposible encontrar BuyPortal en este objeto");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Algo entró al trigger: " + other.name);

        if (!puedeTeletransportar)
        {
            Debug.Log("No se puede teletransportar todavía.");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("El objeto no es el jugador, es: " + other.tag);
            return;
        }

        Debug.Log("Se va a teletransportar al destino: " + destino.name);

        // Desactivar CharacterController antes de mover
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        // Teletransportar posición
        other.transform.position = destino.position;

        // Aplicar rotación (en Y)
        Vector3 nuevaRotacion = other.transform.eulerAngles;
        nuevaRotacion.y = rotacionYAlLlegar;
        other.transform.eulerAngles = nuevaRotacion;

        // Reactivar CharacterController
        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (EstadoPortal == null)
            {
                puedeTeletransportar = true;
                Debug.Log("Portal reactivado.");
            }
            else if (EstadoPortal.PortalComprado()) {
                puedeTeletransportar = true;
            }
            else
            {
                puedeTeletransportar = false;
            }
        }
    }
}
