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

        // Si la flecha no tiene target pero este trigger SÍ tiene nextTarget
        else if (!arrow.HasTarget && nextTarget != null)
        {
            // Activar la flecha con el nuevo destino
            arrow.SetTarget(nextTarget);
        }
        // Si este trigger no tiene destino (nextTarget = null) y la flecha está encendida
        else if (nextTarget == null && arrow.gameObject.activeSelf)
        {
            // Solo apagar si está encendida
            arrow.ClearTarget();
        }
    }
}
