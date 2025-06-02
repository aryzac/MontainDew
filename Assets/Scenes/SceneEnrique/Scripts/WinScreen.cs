using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public string mainMenuScene; // Nombre de la escena del menú principal

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Lock the cursor to the center of the screen

        AudioManager.instance.PlayWinMusic(); // Reproducir la música de victoria al iniciar la pantalla de victoria
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene); // Cargar la escena del menú principal
    }

}
