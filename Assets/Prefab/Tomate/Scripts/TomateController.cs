using StarterAssets;
using UnityEngine;

public class TomateController : MonoBehaviour
{

    [ContextMenu("Recibir daño de prueba")]
    void TestDamage()
    {
        TakeDamage(25);
    }

    private FirstPersonController player; // Referencia al objeto del jugador

    public float moveSpeed; // Velocidad de movimiento del enemigo

    public Rigidbody theRB; // Rigidbody reference

    public Animator anim; // Animator reference

    // Persecucion
    public float chaseRange = 15f, stopCloseRange = 4f; // Rango de persecución y rango de parada del enemigo
    
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
    
    
    public float waitToDisappear = 4f; // Tiempo de espera para desaparecer después de morir

    // División
    public bool splitOnDeath; // Indica si el enemigo se divide al morir
    
    [Header("Split Settings")]
    [SerializeField] private int maxDivisions = 3; // Cuántas veces puede dividirse como máximo
    private float initialScale; // Tamaño inicial del enemigo
    [SerializeField] private float ReductionFactor = 0.75f; // Factor de reducción al dividirse
    private float minSize; // Tamaño mínimo del enemigo
    [SerializeField] private int currentDivision = 0;


    // Saltos
    public float jumpForceMin = 5f; // Altura mínima del salto
    public float jumpForceMax = 8f; // Altura máxima del salto
    public float minJumpTime = 1f; // Tiempo mínimo entre saltos
    public float maxJumpTime = 3f; // Tiempo máximo entre saltos
    private float jumpCounter; // Contador para el salto

    private bool isJumping; // Estado para la animación

    public bool explode; // Indica si el enemigo explota al contacto
    public GameObject explosionEffect; // Prefab de la explosión al tocar al jugador
    public float damageExplotion; // Daño al jugador al tocarlo


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();

        // Patrullaje
        strafeAmount = Random.Range(-.75f, .75f); // Cantidad de movimiento lateral aleatorio del enemigo

        currentHealth = startingHealth; // Inicializar la vida restante al valor inicial

        jumpCounter = Random.Range(minJumpTime, maxJumpTime); // Inicializar el contador de salto

        if (currentDivision == 0)
        {
            initialScale = transform.localScale.x;
            minSize = initialScale * Mathf.Pow(ReductionFactor, maxDivisions);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
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

        jumpCounter -= Time.deltaTime; // Reducir el contador de salto

        if (jumpCounter <=0)
        {
            float jumpForce = Random.Range(jumpForceMin, jumpForceMax);
            theRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Aplicar una fuerza de salto al Rigidbody del enemigo
            // theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, jumpForce, theRB.linearVelocity.z); // Reiniciar la velocidad en el eje Y del Rigidbody para evitar que el enemigo se quede flotando

            jumpCounter = Random.Range(minJumpTime, maxJumpTime); // Reiniciar el contador de salto
        }
        float yStore = theRB.linearVelocity.y; // Almacenar la velocidad en el eje Y del Rigidbody

        float distance = Vector3.Distance(transform.position, player.transform.position); // Calcular la distancia entre el enemigo y el jugador

        if (distance < chaseRange && PlayerController.instance.isDead == false) // Si el jugador está dentro del rango de persecución y el jugador no esa muerto
        {
            //transform.LookAt(player.transform.position);
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z)); // Mirar al jugador en el eje Y

            if (distance > stopCloseRange) // Si el jugador esta en el rango de parada
            {
                theRB.linearVelocity = (transform.forward + (transform.right * strafeAmount))* moveSpeed; // Movimiento del enemigo hacia el jugador con movimiento lateral aleatorio
            }
            else
            {
                theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo cuando está cerca del jugador
            }          
        }
        else
        {
            if (patrolPoints.Length > 0) // Si hay puntos de patrullage definidos
            {
                if(Vector3.Distance(transform.position, new Vector3(patrolPoints[currentPatrolPoint].position.x, transform.position.y, patrolPoints[currentPatrolPoint].position.z)) < .25f) // Si el enemigo está cerca del punto de patrullaje actual
                {
                    // Reducir el contador y detener el movimiento
                    waitCounter -= Time.deltaTime; // Reducir el contador de espera
                    theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo

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
            }
            
        }

        theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, yStore, theRB.linearVelocity.z); // Mantener la velocidad en el eje Y

    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            if(splitOnDeath == true && currentDivision < maxDivisions) // Si el enemigo se divide al morir y el número de divisiones actuales es menor que el máximo permitido
            {
                    startingHealth *= ReductionFactor; // Reducir la vida inicial al dividirse

                    currentHealth = startingHealth; // Reiniciar la vida del enemigo

                    moveSpeed *= ReductionFactor; // Reducir la velocidad de movimiento al dividirse
                    damageExplotion *= ReductionFactor; // Reducir el daño de explosión al dividirse

                    GameObject clone1 = Instantiate(gameObject, transform.position + (transform.right * .5f * transform.localScale.x), Quaternion.identity); // Instanciar un nuevo enemigo en la posición del enemigo actual
                    GameObject clone2 = Instantiate(gameObject, transform.position + (-transform.right * .5f * transform.localScale.x), Quaternion.identity); // Instanciar otro nuevo enemigo en la posición del enemigo actual

                    float newScale = transform.localScale.x * ReductionFactor; // Calcular la nueva escala del enemigo al dividirse
                    clone1.transform.localScale = new Vector3(newScale, newScale, newScale); // Reducir la escala del primer clon
                    clone2.transform.localScale = new Vector3(newScale, newScale, newScale); // Reducir la escala del segundo clon

                    // Aumentar la división actual en los clones
                    TomateController ctrl1 = clone1.GetComponent<TomateController>();
                    TomateController ctrl2 = clone2.GetComponent<TomateController>();
                    ctrl1.currentDivision = this.currentDivision + 1;
                    ctrl2.currentDivision = this.currentDivision + 1;
            }
            anim.SetTrigger("death"); // inicia animación de muerte

            isDead = true; // Marcar al enemigo como muerto

            theRB.linearVelocity = Vector3.zero; // Detener el movimiento del enemigo
            theRB.isKinematic = true; // Hacer que el Rigidbody sea cinemático para que no se vea afectado por la física

            GetComponent<Collider>().enabled = false; // Desactivar el collider del enemigo
        }
    }

    // Explota al tocar al jugador
    void OnCollisionEnter(Collision other)        
    {
        // Verificar si el objeto con el que colisiona es el jugador y si el enemigo está configurado para explotar
        if (other.gameObject.CompareTag("Player") && explode == true)
        {
            // Causar daño al jugador
            // Debug.Log("Enemy Exploded!"); // Imprimir en la consola que el enemigo ha explotado
            Instantiate(explosionEffect, other.transform.position, Quaternion.identity); // Instanciar el efecto de daño en la posición del jugador
            Instantiate(explosionEffect, transform.position, Quaternion.identity); // Instanciar el efecto de daño en la posición del enemigo

            // PlayerHealthController.instance.TakeDamage(damageExplotion); // Llamar al método TakeDamage del controlador de salud del jugador para reducir su salud
            Destroy(gameObject);
        }
    }
}
