using UnityEngine;

public class ArrowLook : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target;

    public float speed = 100f;

    public Transform LookAtTarget => m_Target;
    public bool HasTarget => m_Target != null;

    public void SetTarget(Transform target)
    {
        if (HasTarget) return; // ya tiene objetivo, no cambiar

        m_Target = target;
        gameObject.SetActive(target != null);
    }

    public void ClearTarget()
    {
        m_Target = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!m_Target) return;

        // Rotar hacia el objetivo
        Vector3 direction = m_Target.position - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }
}
