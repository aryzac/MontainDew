using Unity.Collections;
using UnityEngine;

public class WeaponsController : MonoBehaviour
{
    public float range; // Rango de disparo 35
    public Transform cam; // Referencia a la cámara 

    public LayerMask validLayers; // Máscaras de capas válidas para el rayo

    public GameObject impactEffect, damageEffect; // Prefab del efecto de impacto

    public GameObject muzzleFlare; // Prefab del destello de boca de fuego
    public float flareDisplayTime = .1f; // Tiempo de visualización del destello de boca de fuego
    private float flareCounter; // Contador para el tiempo de visualización del destello

    public bool canAutoFire; // Indica si el arma puede disparar automáticamente
    public float timeBetweenShots = .1f; // Tiempo entre disparos automáticos
    private float shotCounter; // Contador para el tiempo entre disparos automáticos

    public int currentAmmo = 100; // Munición actual
    public int clipSize = 15; // Tamaño del cargador
    public int remainingAmmo = 300; // Munición restante

    private UIController UICon; // Referencia al controlador de la interfaz de usuario

    public int pickupAmount; // Munición recogida al recoger un objeto de munición

    public float damageAmount = 15f; // Daño realizado

    public Weapon[] weapons; // Armas disponibles
    private int currentWeapon, previousWeapon; // Índice del arma actual


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UICon = FindFirstObjectByType<UIController>(); // Obtener el controlador de la interfaz de usuario

        SetWeapon(currentWeapon); // Llamar al método para establecer el arma actual

        Reload(); // Llamar al método de recarga para inicializar la munición

    }

    // Update is called once per frame
    void Update()
    {
        if(flareCounter > 0) // Verificar si el contador del destello es mayor que cero
        {
            flareCounter -= Time.deltaTime; // Reducir el contador por el tiempo transcurrido

            if(flareCounter <= 0) // Verificar si el contador ha llegado a cero
            {
                muzzleFlare.SetActive(false); // Desactivar el destello de boca de fuego
            }
        }
        else
        {
            muzzleFlare.SetActive(false); // Desactivar el destello de boca de fuego
        }
    }

    public void Shoot()
    {
        if(currentAmmo > 0) // Verificar si la munición actual es mayor a cero
        {
            //Debug.Log("Disparar"); // Imprimir "Disparar" en la consola
            RaycastHit hit; // Variable para almacenar la información del rayo
            if (Physics.Raycast(cam.position, cam.forward, out hit, range, validLayers)) // Lanzar un rayo desde la cámara hacia adelante
            {
                // Debug.Log(hit.transform.name); // Imprimir el nombre del objeto golpeado en la consola

                if (hit.transform.tag == "Enemy") // Verificar si el objeto golpeado tiene la etiqueta "Enemy"
                {
                    Instantiate(damageEffect, hit.point, Quaternion.identity); // Instanciar el efecto de daño en el punto de colisión

                    hit.transform.GetComponent<EnemyController>().TakeDamage(damageAmount); // Llamar al método TakeDamage del enemigo golpeado

                }
                else
                {
                    Instantiate(impactEffect, hit.point, Quaternion.identity); // Instanciar el efecto de impacto en el punto de colisión

                }

            }

            muzzleFlare.SetActive(true); // Activar el destello de boca de fuego
            flareCounter = flareDisplayTime; // Reiniciar el contador del destello

            shotCounter = timeBetweenShots; // Reiniciar el contador de disparos automáticos

            currentAmmo--; // Reducir la munición actual

            UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munición en la interfaz de usuario

        }

    }

    public void ShootHeld()
    {
        if(canAutoFire) // Verificar si el arma puede disparar automáticamente
        {
            if(shotCounter <= 0) // Verificar si el contador de disparos es menor o igual a cero
            {
                Shoot(); // Llamar al método Shoot para disparar
            }
            else
            {
                shotCounter -= Time.deltaTime; // Reducir el contador de disparos por el tiempo transcurrido
            }
        }
    }

    public void Reload()
    {
        // Debug.Log("Reloading..."); // Imprimir "Reloading..." en la consola

        remainingAmmo += currentAmmo; // Agregar la munición actual a la munición restante

        if (remainingAmmo >= clipSize) // Verificar si la munición restante es mayor o igual al tamaño del cargador
        {
            remainingAmmo -= clipSize; // Reducir la munición restante por el tamaño del cargador
            currentAmmo = clipSize; // Establecer la munición actual al tamaño del cargador
        }
        else
        {
            currentAmmo = remainingAmmo; // Establecer la munición actual a la munición restante
            remainingAmmo = 0; // Establecer la munición restante a cero
        }
        
        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munición en la interfaz de usuario
    }

    public void GetAmmo()
    {
       // Debug.Log("Get Ammo"); // Imprimir "Get Ammo" en la consola

        remainingAmmo += pickupAmount; // Agregar la cantidad de munición recogida a la munición restante

        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munición en la interfaz de usuario

    }

    public void SetWeapon(int weaponToSet)
    {
        if(previousWeapon != currentWeapon) // Verificar si el arma anterior es diferente del arma actual
        {
            weapons[previousWeapon].currentAmmo = currentAmmo; // Guardar la munición actual del arma anterior
            weapons[previousWeapon].remainingAmmo = remainingAmmo; // Guardar la munición restante del arma anterior
        }
        

        range = weapons[weaponToSet].range; // Establecer el rango del arma actual
        flareDisplayTime = weapons[weaponToSet].flareDisplayTime; // Establecer el tiempo de visualización del destello
        canAutoFire = weapons[weaponToSet].canAutoFire; // Establecer si el arma puede disparar automáticamente
        timeBetweenShots = weapons[weaponToSet].timeBetweenShots; // Establecer el tiempo entre disparos automáticos
        currentAmmo = weapons[weaponToSet].currentAmmo; // Establecer la munición actual
        clipSize = weapons[weaponToSet].clipSize; // Establecer el tamaño del cargador
        remainingAmmo = weapons[weaponToSet].remainingAmmo; // Establecer la munición restante
        pickupAmount = weapons[weaponToSet].pickupAmount; // Establecer la cantidad de munición recogida al recoger un objeto de munición
        damageAmount = weapons[weaponToSet].damageAmount; // Establecer el daño realizado

        muzzleFlare = weapons[weaponToSet].muzzleFlare; // Establecer el prefab del destello de boca de fuego

        // Desactivar todas las armas excepto la seleccionada
        foreach (Weapon w in weapons) // Recorrer todas las armas
        {
            w.gameObject.SetActive(false); // Desactivar todas las armas
        }

        weapons[weaponToSet].gameObject.SetActive(true); // Activar el arma seleccionada

        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munición en la interfaz de usuario

        previousWeapon = currentWeapon; // Guardar el índice del arma anterior
    }

    public void NextWeapon()
    {
        currentWeapon++; // Incrementar el índice del arma actual
        if (currentWeapon >= weapons.Length) // Verificar si el índice del arma actual es mayor o igual al número de armas
        {
            currentWeapon = 0; // Reiniciar el índice del arma actual a cero
        }

        SetWeapon(currentWeapon); // Llamar al método para establecer el arma actual
    }

    public void PreviousWeapon()
    {
        currentWeapon--; // Decrementar el índice del arma actual

        if (currentWeapon < 0) // Verificar si el índice del arma actual es menor que cero
        {
            currentWeapon = weapons.Length - 1; // Establecer el índice del arma actual al último índice de armas
        }

        SetWeapon(currentWeapon); // Llamar al método para establecer el arma actual
    }
}
