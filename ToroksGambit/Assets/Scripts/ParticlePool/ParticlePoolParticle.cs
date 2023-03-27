using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolParticle : MonoBehaviour
{
    ParticleSystem pSystem;

    private void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    public void Initialize(float disableTimer, Vector3 newPos, Quaternion newQuad)
    {
        transform.position = newPos;
        transform.rotation = newQuad;
        gameObject.SetActive(true);
        pSystem.Play();
        Invoke("Disable", disableTimer);
        
    }

    public void Disable()
    {
        pSystem.Stop();
        gameObject.SetActive(false);
        
    }

}
