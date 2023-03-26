using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light fireLight;

    [SerializeField, Min(0)] private float maxIntensity = 15;
    [SerializeField, Min(0)] private float minIntensity = 10;

    [SerializeField, Min(0)] float maxFlickerFrequency = 1;
    [SerializeField, Min(0)] float minFlickerFrequency = 0.01f;

    [SerializeField, Min(0)] float strength = 5;

    float baseIntensity;
    private float nextIntensity;

    private float flickerFrequency;
    private float timeOfLastFlicker;

    private float baseRange;

    private void Awake()
    {
        fireLight = GetComponent<Light>();
        baseIntensity = maxIntensity - minIntensity;
        timeOfLastFlicker = Time.time;
        baseRange = fireLight.range;
    }

    private void Update()
    {
        if (timeOfLastFlicker + flickerFrequency < Time.time)
        {
            timeOfLastFlicker = Time.time;
            nextIntensity = Random.Range(minIntensity, maxIntensity);
            flickerFrequency = Random.Range(minFlickerFrequency, maxFlickerFrequency);
        }

        Flicker();
    }

    private void Flicker()
    {
        fireLight.intensity = Mathf.Lerp(fireLight.intensity, nextIntensity, strength * Time.deltaTime);
        //fireLight.range = (fireLight.intensity / baseIntensity) * baseRange; 
    }

    public void Reset()
    {
        fireLight.intensity = baseIntensity;
    }
}
