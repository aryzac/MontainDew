using UnityEngine;

public class AnimalPatrolController : MonoBehaviour
{
    private PlayerController player; // Referencia al objeto del jugador

    public float moveSpeed; // Velocidad de movimiento del enemigo

    public Rigidbody theRB; // Rigidbody reference

    public Animator anim; // Animator reference

    // Patrullaje
    private float strafeAmount; // Cantidad de movimiento lateral aleatorio del enemigo
    public Transform[] patrolPoints; // Puntos de patrullaje del enemigo
    [HideInInspector] public int currentPatrolPoint; // Índice del punto de patrullaje actual del enemigo
    public Transform pointsHolder; // Objeto padre que contiene los puntos de patrullaje
    public float pointWaitTime = 3f; // Tiempo de espera en cada punto de patrullaje
    private float waitCounter; // Contador de espera en cada punto de patrullaje

    
    // Daño
    private bool isDead; // Indica si el enemigo está muerto
    public float currentHealth = 25f; // Vida restante
    public float startingHealth = 25f; // Vida inicial
    
    
    public float waitToDisappear = 3f; // Tiempo de espera para desaparecer después de morir

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>(); // Encontrar el objeto del jugador en la escena

        // Patrullaje
        strafeAmount = Random.Range(-.75f, .75f); // Cantidad de movimiento lateral aleatorio del enemigo
        pointsHolder.SetParent(null); // Obtener el objeto padre que contiene los puntos de patrullaje
        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime; // Inicializar el contador de espera en un rango aleatorio entre 0.75 y 1.25 segundos

        currentHealth = startingHealth; // Inicializar la vida restante al valor inicial
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == true)
        {
            waitToDisappear -= Time.deltaTime; // Reducir el contador de espera para desaparecer

            if(waitToDisappear <= 0) // Si el contador ha llegado a cero
            {
                // se reduce la escala del enemigo
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime); // Hacer que el enemigo desaparezca lentamente

                if(transform.localScale.x <= .1f) // Si la escala del enemigo es menor o igual a 0.1
                {
                    Destroy(gameObject); // Destruir el objeto del enemigo
                }
            }
            return; // Si el enemigo está muerto, no hacer nada y termina el update
        }

        float yStore = theRB.linearVelocity.y; // Almacenar la velocidad en el eje Y del Rigidbody

        //if (PlayerController.instance.isDead == false) // Si el jugador está dentro del rango de persecución y el jugador no esa muerto
        //{
            if (patrolPoints.Length > 0) // Si hay puntos de patrullage definidos
            {
                if(Vector3.Distance(transform.position, new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)) < .25f) // Si el enemigo está cerca del punto de patrullaje actual
                {
                    // Reducir el contador y detener el movimiento
                    waitCounter -= Time.deltaTime; // Reducir el contador de espera
                    theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo
                    anim.SetBool("Move", false); // Hacer que la animación de caminar se detenga cuando el enemigo no se mueva
                    anim.SetTrigger("Mirar") ; // Hacer que el enemigo mire al siguiente punto de patrullaje cuando está esperando

                    if (waitCounter <= 0) // Si el contador de espera ha llegado a cero
                    {
                        // Cambiar al siguiente punto de patrullaje
                        currentPatrolPoint++; // Cambiar al siguiente punto de patrullaje
                        if (currentPatrolPoint >= patrolPoints.Length) // Si se ha llegado al último punto de patrullaje
                        {
                            currentPatrolPoint = 0; // Volver al primer punto de patrullaje
                        }

                        waitCounter = Random.Range(.75f, 1.25f) * pointWaitTime; // Reiniciar el contador de espera en un rango aleatorio entre 0.75 y 1.25 segundos
                    }
                    
                }
                else
                {
                    transform.LookAt(new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)); // Mirar al siguiente punto de patrullaje

                    theRB.linearVelocity = transform.forward * moveSpeed; // Movimiento del enemigo hacia el siguiente punto de patrullaje

                    anim.SetBool("Move", true); // Hacer que la animación de caminar se reproduzca cuando el enemigo se mueva     
                }
            }
            else
            {
                theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo cuando está fuera del rango de persecución

                anim.SetBool("Move", false); // Hacer que la animación de caminar se detenga cuando el enemigo no se mueva
            }
            
        //}

        theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, yStore, theRB.linearVelocity.z); // Mantener la velocidad en el eje Y

    }

    public void TakeDamage(float damageToTake)
    {
        // Debug.Log("Enemy hit"); // Imprimir un mensaje en la consola cuando el enemigo recibe daño

        // Destroy(gameObject); // Destruir el objeto del enemigo

        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            anim.SetTrigger("death"); // inicia animación de muerte

            isDead = true; // Marcar al enemigo como muerto

            theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo
            theRB.isKinematic = true; // Hacer que el Rigidbody sea cinemático para que no se vea afectado por la física

            GetComponent<Collider>().enabled = false; // Desactivar el collider del enemigo
        }
    }
}
