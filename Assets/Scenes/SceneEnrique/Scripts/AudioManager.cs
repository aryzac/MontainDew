using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if(instance == null) // Si no hay otra instancia de AudioManager
        {
            instance = this;

            DontDestroyOnLoad(gameObject); // No destruir este objeto al cargar una nueva escena
        }
        else
        {
            Destroy(gameObject); // Destruir este objeto si ya hay una instancia
        }
        
    }

    public AudioSource titleMusic, levelMusic, bossMusic, winMusic; // Referencias a los diferentes AudioSources

    public AudioSource[] sfx; // Array para almacenar efectos de sonido

    public void PlayTitleMusic()
    {
        StopAllMusic();
        titleMusic.Play(); // Reproducir la música del título
    }

    public void PlayLevelMusic()
    {
        StopAllMusic();
        levelMusic.Play(); // Reproducir la música del nivel
    }

    public void PlayBossMusic()
    {
        StopAllMusic();
        bossMusic.Play(); // Reproducir la música del jefe
    }

    public void PlayWinMusic()
    {
        StopAllMusic();
        winMusic.Play(); // Reproducir la música de victoria
    }

    public void StopAllMusic()
    {
        titleMusic.Stop(); // Detener la música del título
        levelMusic.Stop(); // Detener la música del nivel
        bossMusic.Stop(); // Detener la música del jefe
        winMusic.Stop(); // Detener la música de victoria
    }

    public void PlaySFX(int sfxToPlay)
    {
        sfx[sfxToPlay].Stop(); // Detener el efecto de sonido actual si está reproduciéndose


        sfx[sfxToPlay].Play(); // Reproducir el efecto de sonido correspondiente

    }
}
