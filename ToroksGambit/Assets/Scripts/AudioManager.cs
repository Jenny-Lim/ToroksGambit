using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;

    public static AudioManager instance;
    private float overallVolume;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetVolume(SaveManager.instance.savedVolume);
    }

    public void SetVolume(float volume)
    {
        instance.masterMixer.SetFloat("masterVolume",volume);
    }
}
