using UnityEngine;
using UnityEngine.UI; // Importar el espacio de nombres para UI

public class MainMenu : MonoBehaviour
{
    public string firstLevel; // Nombre de la escena del primer nivel

    private void Start()
    {
        AudioManager.instance.PlayTitleMusic(); // Reproducir la música del título al iniciar el menú
    }
    public void StartGame()
    {
        // Cargar la escena del juego
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);

        // Reproducir la música del nivel
        AudioManager.instance.PlayLevelMusic();
    }

    public void QuitGame()
    {
        // Salir del juego
        Application.Quit();

        Debug.Log("Game is exiting..."); // Imprimir en la consola que el juego está saliendo

        // Si estamos en el editor, detener la ejecución
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
