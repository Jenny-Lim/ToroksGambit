using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class UIParticleSystem : MonoBehaviour
{
    struct SpawnBox {
        public Vector3 center;
        public Vector3 bounds;

        public SpawnBox(Vector3 newCenter, Vector3 newBounds)
        {
            center = newCenter;
            bounds = newBounds;
        }
    };

    public static UIParticleSystem instance;

    [Tooltip("The object that the tickets will move towards")]
    [SerializeField] Transform target;
    [Space(4)]

    [SerializeField] private GameObject particlePrefab;//holds the prefab that represents a particle
    private List<UIParticle> particles;//the list of all active particles
    [Space(4)]

    public bool isEmitting = false;//whether to emit more particles, if false does not delete/remove particles see StopAndClear()
    [Space(4)]

    [SerializeField] private int maxParticles = -1;//the maximum number of particles that can be active at once, -1 means infinite
    [SerializeField] private float spawnDelay = 1;//how much time to wait before trying to spawn another particle
    [Space(4)]

    private SpawnBox spawnArea;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaBounds;
    [Space(4)]

    [SerializeField] private int minSpawnAmount = 1;//the minimum number of particles that can spawn when spawning (does NOT go over maxParticles)
    [SerializeField] private int maxSpawnAmount = 3;//the maximum number of particles that can spawn when spawning (does NOT go over maxParticles)
    [Space(4)]

    [SerializeField] private float maxParticleSpeed = 20;
    [SerializeField] private float minParticleSpeed = 1;
    [SerializeField] private float scaleSpeed;

    [SerializeField] private AudioClip ticketPopSound;

    private int particleCount { get { return particles.Count; } }//returns the number of active particles 


    private void Awake()
    {
        if (instance == null) { instance = this; }

        if (maxParticles > -1)
        {
            particles = new List<UIParticle>(maxParticles);
        }
        else
        {
            particles = new List<UIParticle>(20);//default amount
        }
        spawnArea = new SpawnBox(spawnAreaCenter, spawnAreaBounds);

        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isEmitting)
        {
            //emit particle
        }

        if (particleCount > 0)
        {
            UpdateParticles(Time.deltaTime, target);
        }
    }

    public void Emit()
    {
        isEmitting = true;
    }
    public void Stop()
    {
        isEmitting = false;
    }

    public void StopAndClear()
    {
        isEmitting = false;
        particles.Clear();
    }

    private void DeleteOldestParticle()
    {

    }

    public void DeleteParticle(int index)
    {

    }

    public void DeleteParticle()
    {

    }

    public void DeleteParticle(GameObject particle)
    {
        for (int i =  0; i < particleCount; i++)
        {
            if (particle == particles[i].gameObject)
            {
                particles.RemoveAt(i);
                Destroy(particle);
                return;
            }
        }

        Debug.LogError("Tried to remove particle that did not exist inside list");
    }

    private void SpawnParticle()
    {
        if (particleCount >= maxParticles) { return; }
        UIParticle particle = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<UIParticle>();
        InitParticle(ref particle);
        particles.Add(particle);
        //SoundObjectPool.instance.GetPoolObject().Play(ticketPopSound);
    }

    private Vector3 FindSpawnLocation()
    {
        Vector3 pos = new Vector3();

        pos.x = (int)Random.Range(spawnArea.center.x - spawnAreaBounds.x, spawnArea.center.x + spawnAreaBounds.x);
        pos.y = (int)Random.Range(spawnArea.center.y - spawnAreaBounds.y, spawnArea.center.y + spawnAreaBounds.y);
        pos.z = (int)Random.Range(spawnArea.center.z - spawnAreaBounds.z, spawnArea.center.z + spawnAreaBounds.z);

        return pos;
    }

    private Vector2 RandomParticleVelocity()
    {
        Vector2 vel = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        vel *= maxParticleSpeed;
        vel.x = Mathf.Max(vel.x, minParticleSpeed);
        vel.y = Mathf.Max(vel.y, minParticleSpeed);
        return vel;
    }

    private void UpdateParticles(float deltaTime, Transform moveTowards)
    {
        foreach (UIParticle particle in particles)
        {
            particle.UpdateParticle(deltaTime, moveTowards);
        }
    }

    private void InitParticle(ref UIParticle particle)
    {
        particle.transform.localPosition = FindSpawnLocation();
        particle.SetVelocity(RandomParticleVelocity());
        particle.transform.localScale = Vector3.zero;
        particle.scaleSpeed = scaleSpeed;
    }

    public void SpawnTickets(int amount)
    {
        StartCoroutine(SpawnTicketsCoRo(amount));
    }

    private IEnumerator SpawnTicketsCoRo(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnParticle();
            yield return new WaitForSeconds(spawnDelay);
        }

        /*while (particleCount < amount)
        {
            int rand = (int)Random.Range(minSpawnAmount, maxSpawnAmount + 0.99f);
            for (int i = 0; i < rand; i++)
            {
                SpawnParticle();
                yield return new WaitForSeconds(spawnDelay);
            }
            
        }*/
    }
}
