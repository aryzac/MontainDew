using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    float rotationX = 0f;
    float rotationY = 0f;

    void Start()
    {
        // Oculta y bloquea el cursor al centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Movimiento con teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float upDown = 0f;

        if (Input.GetKey(KeyCode.E)) upDown = 1f;
        if (Input.GetKey(KeyCode.Q)) upDown = -1f;

        Vector3 moveDirection = transform.right * horizontal + transform.up * upDown + transform.forward * vertical;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotación con mouse
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Limita la rotación vertical

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
