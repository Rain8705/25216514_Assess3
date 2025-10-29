using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip bgmNormal;
    public AudioClip bgmScared;
    public AudioClip bgmDead;

    [Header("SFX")]
    public AudioClip sfxMove;
    public AudioClip sfxEatPellet;
    public AudioClip sfxDeath;
    public AudioClip sfxCollideWall;

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!bgmSource || !clip) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip);
    }
}
