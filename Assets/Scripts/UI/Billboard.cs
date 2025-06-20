using UnityEngine;
using Cinemachine;

/// <summary>
/// Billboard que siempre mira hacia la cámara real (CinemachineBrain o Camera.main),
/// rotando solo en el eje Y y preservando escala.
/// </summary>
public class Billboard : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 initialScale;

    void Awake()
    {
        // Guardar escala original
        initialScale = transform.localScale;

        // Buscar el CinemachineBrain (está en el Main Camera)
        var brain = Object.FindObjectOfType<CinemachineBrain>();
        if (brain != null)
        {
            camTransform = brain.transform;  // la cámara real que renderiza
        }
        else if (Camera.main != null)
        {
            // Fallback a la cámara principal si no hay Brain
            camTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Billboard: no se encontró CinemachineBrain ni Camera.main.");
        }
    }

    void LateUpdate()
    {
        if (camTransform == null)
            return;

        // Dirección plana (XZ) hacia la cámara real
        Vector3 dir = camTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Calcular ángulo Y y sumar 180° para front-facing
        float angleY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.Euler(0f, angleY, 0f);

        // Restaurar escala original
        transform.localScale = initialScale;
    }
}
