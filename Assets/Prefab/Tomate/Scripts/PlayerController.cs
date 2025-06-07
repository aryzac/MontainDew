using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // Singleton est�tico para asegurar que solo haya una instancia de PlayerController
    public static PlayerController instance; // Instancia est�tica de PlayerController
    private void Awake()
    {
        instance = this; // Asignar la instancia si no existe
    }

    // Referencia al componente Character Controller
    private CharacterController charCon; // Componente Character Controller
    private Vector3 currentMovement; // Movimiento actual del personaje
    private Vector2 rotStore; // Almacenar la rotaci�n del personaje


    public bool isDead; // Indica si el jugador est� muerto


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = false;
}

    // Update is called once per frame
    void Update()
    {
        if(isDead == true) // Verificar si el jugador est� muerto
        {
            return; // Salir del m�todo si el jugador est� muerto
        }

        // 
        if(Time.timeScale == 0) // Verificar si el tiempo est� pausado
        {
            return; // Salir del m�todo si el tiempo est� pausado
        }   
    }

}
