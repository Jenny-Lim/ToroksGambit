using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class SoundObject : MonoBehaviour
{

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public float Play(AudioClip newClip)
    {
        if (newClip == null) return 0;
        source.clip = newClip;
        source.Play();
        Invoke("Deactivate", source.clip.length);
        return source.clip.length;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        source.Stop();
        gameObject.SetActive(false);
    }

}
