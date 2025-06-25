using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour, ISoundController
{
    
    public static SoundController Instance { get; private set; }
    
    [SerializeField] private AudioSource audioSourceOnce;
    [SerializeField] private AudioSource audioSourceLoop;
    [SerializeField] private AudioClip defaultMusic;
 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            float vol = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
            
            audioSourceOnce.volume = audioSourceLoop.volume = vol;
            
            // Si no estaba sonando nada, arranco la música por defecto:
            if (audioSourceLoop.clip == null || !audioSourceLoop.isPlaying)
                PlaySoundLoop(defaultMusic);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // if (audioSourceOnce != null)
        // {
        //     audioSourceOnce.volume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
        // }
        // if (audioSourceLoop != null)
        // {
        //     audioSourceLoop.volume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
        // }
        UpdateVolume(GameManager.Instance.GetVolume());
    }

    private void Update()
    {
        if (audioSourceOnce != null && audioSourceLoop != null)
        {
            float v = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
            audioSourceOnce.volume = audioSourceLoop.volume = v;
        }
        // audioSourceOnce.volume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
        // audioSourceLoop.volume = PlayerPrefs.GetFloat("MasterVolume", 0.3f);
    }
    
    public void UpdateVolume(float v)
    {
        audioSourceOnce.volume = audioSourceLoop.volume = v;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSourceOnce.loop = false;
        audioSourceOnce.PlayOneShot(clip);
    }

    public void PlaySoundLoop(AudioClip clip)
    {
        audioSourceLoop.loop = true;
        audioSourceLoop.clip = clip;
        audioSourceLoop.Play();
    }

    public void StopLoopSound()
    {
        audioSourceLoop.Stop();
    }
}
