using UnityEngine;
using UnityEngine.SceneManagement; // Importar el espacio de nombres para la gestión de escenas

public class LevelExit : MonoBehaviour
{
    public string sceneToLoad; // Nombre de la escena a cargar al salir

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") // Verificar si el objeto que colisiona tiene la etiqueta "Player"
        {
            SceneManager.LoadScene(sceneToLoad); // Llamar al método para cargar la escena especificada

            AudioManager.instance.PlayLevelMusic(); // Reproducir la música del nivel al entrar en la salida

            AudioManager.instance.PlaySFX(3); // Reproducir el efecto de sonido de salida

        }
    }
}
