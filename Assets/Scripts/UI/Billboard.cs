using UnityEngine;
using Cinemachine;

/// <summary>
/// Billboard que siempre mira hacia la c�mara real (CinemachineBrain o Camera.main),
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

        // Buscar el CinemachineBrain (est� en el Main Camera)
        var brain = Object.FindObjectOfType<CinemachineBrain>();
        if (brain != null)
        {
            camTransform = brain.transform;  // la c�mara real que renderiza
        }
        else if (Camera.main != null)
        {
            // Fallback a la c�mara principal si no hay Brain
            camTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Billboard: no se encontr� CinemachineBrain ni Camera.main.");
        }
    }

    void LateUpdate()
    {
        if (camTransform == null)
            return;

        // Direcci�n plana (XZ) hacia la c�mara real
        Vector3 dir = camTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Calcular �ngulo Y y sumar 180� para front-facing
        float angleY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.Euler(0f, angleY, 0f);

        // Restaurar escala original
        transform.localScale = initialScale;
    }
}
