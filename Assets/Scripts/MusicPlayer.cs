using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour {

    public AudioClip[] Songs;
    private float Volume;
    private AudioSource audSrc;
    public static MusicPlayer instance;

    private int currentTrack;
    private bool paused;

    private void Start()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        paused = false;
        audSrc = gameObject.AddComponent<AudioSource>();

        audSrc.loop = false;

        currentTrack = Random.Range(0, Songs.Length);
        audSrc.clip = Songs[currentTrack];
        audSrc.Play();

        StartCoroutine(WaitForEnd());
    }

    public void ChangeVolume(float volume)
    {
        audSrc.volume = volume;
    }

    public void ToggleOn(bool t)
    {
        paused = !t;

        if (paused)
            audSrc.Pause();
        else
        {
            audSrc.UnPause();
            StartCoroutine(WaitForEnd());
        }
    }

    private IEnumerator WaitForEnd()
    {
        while (audSrc.isPlaying && !paused)
        {
            yield return new WaitForSeconds(0.02f);
        }
        if (!paused)
        {
            ChangeTrack((currentTrack + 1) % Songs.Length);
        }
    }

    private void ChangeTrack(int clipNo)
    {
        audSrc.Stop();
        audSrc.clip = Songs[clipNo];
        currentTrack = clipNo;
        audSrc.Play();
        StartCoroutine(WaitForEnd());
    }
}
