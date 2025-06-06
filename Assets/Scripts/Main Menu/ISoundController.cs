using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundController
{
    void PlaySound(AudioClip clip);
    void PlaySoundLoop(AudioClip clip);
    void StopLoopSound();
}
