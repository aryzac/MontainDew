using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range; // Rango de disparo 35

    public GameObject muzzleFlare; // Prefab del destello de boca de fuego
    public float flareDisplayTime = .1f; // Tiempo de visualización del destello de boca de fuego


    public bool canAutoFire; // Indica si el arma puede disparar automáticamente
    public float timeBetweenShots = .1f; // Tiempo entre disparos automáticos

    public int currentAmmo = 100; // Munición actual
    public int clipSize = 15; // Tamaño del cargador
    public int remainingAmmo = 300; // Munición restante

    public int pickupAmount; // Munición recogida al recoger un objeto de munición

    public float damageAmount = 15f; // Daño realizado

    public int sfxIndex; // Índice del efecto de sonido del disparo
}
