using UnityEngine;

public class ArrowTriggerZone : MonoBehaviour
{
    [Header("Referencia a la flecha")]
    public ArrowLook arrow;

    [Header("Destino siguiente (dejar vacío para apagar)")]
    public Transform nextTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (arrow == null) return;
        // Asegurarse de que la flecha está activa
        if (!arrow.gameObject.activeSelf)
        {
            arrow.gameObject.SetActive(true);
        }
        Debug.Log(arrow.HasTarget + " - " + arrow.LookAtTarget + " - " + transform.name);

        // Si este trigger es el objetivo actual
        if (arrow.HasTarget && arrow.LookAtTarget == transform)
        {
            if (nextTarget != null)
            {
                // Hay un siguiente destino: cambiarlo
                arrow.ClearTarget(); // Asegurarse de dejar de mirar este
                arrow.SetTarget(nextTarget);
            }
            else
            {
                // No hay destino siguiente ? apagar
                arrow.ClearTarget();
            }
        }

        // ?? Caso especial: si la flecha está apagada, iniciar destino inicial (ej. primer trigger)
        if (!arrow.HasTarget && nextTarget != null)
        {
            arrow.SetTarget(nextTarget);
        }
    }
}
