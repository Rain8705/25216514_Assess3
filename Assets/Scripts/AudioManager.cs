using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioSource bgmSource;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip sfxMove;
    public AudioClip sfxEatPellet;
    public AudioClip sfxCollideWall;
    public AudioClip sfxDeath;

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip != null && bgmSource != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
}
