using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // Singleton estático para asegurar que solo haya una instancia de PlayerController
    public static PlayerController instance; // Instancia estática de PlayerController
    private void Awake()
    {
        instance = this; // Asignar la instancia si no existe
    }

    // Referencia al componente Character Controller
    private CharacterController charCon; // Componente Character Controller
    private Vector3 currentMovement; // Movimiento actual del personaje
    private Vector2 rotStore; // Almacenar la rotación del personaje

    public InputActionReference moveAction; // Referencia a la acción de movimiento
    public InputActionReference lookAction; // Referencia a la acción de mirar
    public InputActionReference jumpAction; // Referencia a la acción de saltar
    public InputActionReference SprintAction; // Referencia a la acción de correr
    public InputActionReference shootAction; // Referencia a la acción de disparar
    public InputActionReference reloadAction; // Referencia a la acción de recargar
    public Camera theCam; // Referencia a la cámara
    public WeaponsController weaponCon; // Referencia al controlador de armas
    public InputActionReference nextWeapon, prevWeapon; // Referencias a las acciones de cambiar de arma



    public float moveSpeed = 5f; // Velocidad de movimiento

    public float lookSpeed = 2f; // Velocidad de mirar
    public float minLookAngle, maxLookAngle; // Ángulos mínimo y máximo de mirada -85 y 75

    public float jumpPower; // Potencia de salto
    public float gravityModifier = 4f; // Gravedad

    public float runSpeed = 10f; // Velocidad de carrera
    public float camZoomNormal, camZoomOut, camZoomSpeed; // Zoom normal y zoom out de la cámara al correr 60 y 75 y velocidad de zoom 5f

    public bool isDead; // Indica si el jugador está muerto




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charCon = GetComponent<CharacterController>(); // Obtener el componente Character Controller
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor en el centro de la pantalla

    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == true) // Verificar si el jugador está muerto
        {
            return; // Salir del método si el jugador está muerto
        }

        // 
        if(Time.timeScale == 0) // Verificar si el tiempo está pausado
        {
            return; // Salir del método si el tiempo está pausado
        }
        
        // Capturar el movimiento del personaje
        float yStore = currentMovement.y; // Almacenar el valor de Y del movimiento actual

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>(); // Leer el valor de entrada de movimiento

        // Debug.Log(moveInput); // Imprimir el valor de entrada en la consola
                
        Vector3 moveFoward = transform.forward * moveInput.y; // Calcular el movimiento hacia adelante basado en la direccion del personaje
        Vector3 moveSideways = transform.right * moveInput.x; // Calcular el movimiento lateral basado en la direccion del personaje


        // Combinar el movimiento hacia adelante y lateral y aplicar la velocidad
        // Verificar si se está corriendo
        if (SprintAction.action.IsPressed()) // Verificar si la acción de correr está presionada
        {
            currentMovement = (moveFoward + moveSideways) * runSpeed; // Aumentar la velocidad de movimiento

            if(currentMovement != Vector3.zero) // Verificar si hay movimiento
            {
                theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, camZoomOut, camZoomSpeed * Time.deltaTime); // Aumentar el campo de visión de la cámara al correr
            }
            
        }
        else
        {
            currentMovement = (moveFoward + moveSideways) * moveSpeed; // Mantener la velocidad de movimiento normal

            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, camZoomNormal, camZoomSpeed * Time.deltaTime); // Restablecer el campo de visión de la cámara al valor normal
        }

               
        if(charCon.isGrounded) // Verificar si el personaje está en el suelo
        {
            yStore = 0f; // Reiniciar la gravedad si está en el suelo
        }
        
        currentMovement.y = yStore + (Physics.gravity.y * Time.deltaTime * gravityModifier); // Aplicar la gravedad al movimiento actual

        // Aplicar el salto si se presiona la tecla de salto
        if(jumpAction.action.WasPressedThisFrame() && charCon.isGrounded == true)
        {
            currentMovement.y = jumpPower; // Aplicar la potencia de salto al movimiento actual
        }

        // Aplicar el movimiento al CharacterController
        charCon.Move(currentMovement * Time.deltaTime); // Mover el CharacterController

        //// Aplicar el movimiento al CharacterController
        //Vector3 move = new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed); // Crear un vector de movimiento
        //charCon.Move(move * Time.deltaTime); // Mover el CharacterController

        // Mover el personaje hacia adelante en cada fotograma
        //charCon.Move(new Vector3(0, 0, moveSpeed) * Time.deltaTime);

        // Manejar la rotación del personaje
        // Capturar la entrada de la rotración
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>(); // Leer el valor de entrada de mirar
        lookInput.y = -lookInput.y; // Invertir el eje Y
        //Acumular la rotación con el input multiplicado por la velocidad y el tiempo
        rotStore += lookInput * lookSpeed * Time.deltaTime; // Acumular la rotación
        
        rotStore.y = Mathf.Clamp(rotStore.y, minLookAngle, maxLookAngle); // Limitar la rotación en el eje Y
        
        transform.rotation = Quaternion.Euler(0, rotStore.x, 0f); // Rotar el personaje en el eje Y
        theCam.transform.localRotation = Quaternion.Euler(rotStore.y, 0f, 0f); // Rotar la cámara en el eje X


        // Disparar el arma
        if (shootAction.action.WasPressedThisFrame()) // Verificar si la acción de disparar fue presionada
        {
            weaponCon.Shoot(); // Llamar al método Shoot del controlador de armas
        }

        if (shootAction.action.IsPressed()) // Verificar si la acción de disparar está presionada
        {
            weaponCon.ShootHeld(); // Llamar al método ShootHeld del controlador de armas
        }

        // Recargar el arma
        if (reloadAction.action.WasPressedThisFrame()) // Verificar si la acción de recargar fue presionada
        {
            weaponCon.Reload(); // Llamar al método Reload del controlador de armas
        }
        
        // Cambiar de arma
        if(nextWeapon.action.WasPressedThisFrame()) // Verificar si la acción de cambiar a la siguiente arma fue presionada
        {
            weaponCon.NextWeapon(); // Llamar al método NextWeapon del controlador de armas
        }

        if(prevWeapon.action.WasPressedThisFrame()) // Verificar si la acción de cambiar a la arma anterior fue presionada
        {
            weaponCon.PreviousWeapon(); // Llamar al método PrevWeapon del controlador de armas
        }
    }

}
