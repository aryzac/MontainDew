using Unity.Collections;
using UnityEngine;

public class WeaponsController : MonoBehaviour
{
    public float range; // Rango de disparo 35
    public Transform cam; // Referencia a la c�mara 

    public LayerMask validLayers; // M�scaras de capas v�lidas para el rayo

    public GameObject impactEffect, damageEffect; // Prefab del efecto de impacto

    public GameObject muzzleFlare; // Prefab del destello de boca de fuego
    public float flareDisplayTime = .1f; // Tiempo de visualizaci�n del destello de boca de fuego
    private float flareCounter; // Contador para el tiempo de visualizaci�n del destello

    public bool canAutoFire; // Indica si el arma puede disparar autom�ticamente
    public float timeBetweenShots = .1f; // Tiempo entre disparos autom�ticos
    private float shotCounter; // Contador para el tiempo entre disparos autom�ticos

    public int currentAmmo = 100; // Munici�n actual
    public int clipSize = 15; // Tama�o del cargador
    public int remainingAmmo = 300; // Munici�n restante

    private UIController UICon; // Referencia al controlador de la interfaz de usuario

    public int pickupAmount; // Munici�n recogida al recoger un objeto de munici�n

    public float damageAmount = 15f; // Da�o realizado

    public Weapon[] weapons; // Armas disponibles
    private int currentWeapon, previousWeapon; // �ndice del arma actual


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UICon = FindFirstObjectByType<UIController>(); // Obtener el controlador de la interfaz de usuario

        SetWeapon(currentWeapon); // Llamar al m�todo para establecer el arma actual

        Reload(); // Llamar al m�todo de recarga para inicializar la munici�n

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
        if(currentAmmo > 0) // Verificar si la munici�n actual es mayor a cero
        {
            //Debug.Log("Disparar"); // Imprimir "Disparar" en la consola
            RaycastHit hit; // Variable para almacenar la informaci�n del rayo
            if (Physics.Raycast(cam.position, cam.forward, out hit, range, validLayers)) // Lanzar un rayo desde la c�mara hacia adelante
            {
                // Debug.Log(hit.transform.name); // Imprimir el nombre del objeto golpeado en la consola

                if (hit.transform.tag == "Enemy") // Verificar si el objeto golpeado tiene la etiqueta "Enemy"
                {
                    Instantiate(damageEffect, hit.point, Quaternion.identity); // Instanciar el efecto de da�o en el punto de colisi�n

                    hit.transform.GetComponent<EnemyController>().TakeDamage(damageAmount); // Llamar al m�todo TakeDamage del enemigo golpeado

                }
                else
                {
                    Instantiate(impactEffect, hit.point, Quaternion.identity); // Instanciar el efecto de impacto en el punto de colisi�n

                }

            }

            muzzleFlare.SetActive(true); // Activar el destello de boca de fuego
            flareCounter = flareDisplayTime; // Reiniciar el contador del destello

            shotCounter = timeBetweenShots; // Reiniciar el contador de disparos autom�ticos

            currentAmmo--; // Reducir la munici�n actual

            UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munici�n en la interfaz de usuario

        }

    }

    public void ShootHeld()
    {
        if(canAutoFire) // Verificar si el arma puede disparar autom�ticamente
        {
            if(shotCounter <= 0) // Verificar si el contador de disparos es menor o igual a cero
            {
                Shoot(); // Llamar al m�todo Shoot para disparar
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

        remainingAmmo += currentAmmo; // Agregar la munici�n actual a la munici�n restante

        if (remainingAmmo >= clipSize) // Verificar si la munici�n restante es mayor o igual al tama�o del cargador
        {
            remainingAmmo -= clipSize; // Reducir la munici�n restante por el tama�o del cargador
            currentAmmo = clipSize; // Establecer la munici�n actual al tama�o del cargador
        }
        else
        {
            currentAmmo = remainingAmmo; // Establecer la munici�n actual a la munici�n restante
            remainingAmmo = 0; // Establecer la munici�n restante a cero
        }
        
        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munici�n en la interfaz de usuario
    }

    public void GetAmmo()
    {
       // Debug.Log("Get Ammo"); // Imprimir "Get Ammo" en la consola

        remainingAmmo += pickupAmount; // Agregar la cantidad de munici�n recogida a la munici�n restante

        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munici�n en la interfaz de usuario

    }

    public void SetWeapon(int weaponToSet)
    {
        if(previousWeapon != currentWeapon) // Verificar si el arma anterior es diferente del arma actual
        {
            weapons[previousWeapon].currentAmmo = currentAmmo; // Guardar la munici�n actual del arma anterior
            weapons[previousWeapon].remainingAmmo = remainingAmmo; // Guardar la munici�n restante del arma anterior
        }
        

        range = weapons[weaponToSet].range; // Establecer el rango del arma actual
        flareDisplayTime = weapons[weaponToSet].flareDisplayTime; // Establecer el tiempo de visualizaci�n del destello
        canAutoFire = weapons[weaponToSet].canAutoFire; // Establecer si el arma puede disparar autom�ticamente
        timeBetweenShots = weapons[weaponToSet].timeBetweenShots; // Establecer el tiempo entre disparos autom�ticos
        currentAmmo = weapons[weaponToSet].currentAmmo; // Establecer la munici�n actual
        clipSize = weapons[weaponToSet].clipSize; // Establecer el tama�o del cargador
        remainingAmmo = weapons[weaponToSet].remainingAmmo; // Establecer la munici�n restante
        pickupAmount = weapons[weaponToSet].pickupAmount; // Establecer la cantidad de munici�n recogida al recoger un objeto de munici�n
        damageAmount = weapons[weaponToSet].damageAmount; // Establecer el da�o realizado

        muzzleFlare = weapons[weaponToSet].muzzleFlare; // Establecer el prefab del destello de boca de fuego

        // Desactivar todas las armas excepto la seleccionada
        foreach (Weapon w in weapons) // Recorrer todas las armas
        {
            w.gameObject.SetActive(false); // Desactivar todas las armas
        }

        weapons[weaponToSet].gameObject.SetActive(true); // Activar el arma seleccionada

        UICon.UpdateAmmoText(currentAmmo, remainingAmmo); // Actualizar el texto de munici�n en la interfaz de usuario

        previousWeapon = currentWeapon; // Guardar el �ndice del arma anterior
    }

    public void NextWeapon()
    {
        currentWeapon++; // Incrementar el �ndice del arma actual
        if (currentWeapon >= weapons.Length) // Verificar si el �ndice del arma actual es mayor o igual al n�mero de armas
        {
            currentWeapon = 0; // Reiniciar el �ndice del arma actual a cero
        }

        SetWeapon(currentWeapon); // Llamar al m�todo para establecer el arma actual
    }

    public void PreviousWeapon()
    {
        currentWeapon--; // Decrementar el �ndice del arma actual

        if (currentWeapon < 0) // Verificar si el �ndice del arma actual es menor que cero
        {
            currentWeapon = weapons.Length - 1; // Establecer el �ndice del arma actual al �ltimo �ndice de armas
        }

        SetWeapon(currentWeapon); // Llamar al m�todo para establecer el arma actual
    }
}
