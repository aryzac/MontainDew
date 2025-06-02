using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FindFirstObjectByType<WeaponsController>().GetAmmo(); // Llamar al m�todo GetAmmo() en WeaponsController

            Destroy(gameObject);

            AudioManager.instance.PlaySFX(4); // Reproducir el efecto de sonido de recogida de munici�n
        }
    }
}
