using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobObject : MonoBehaviour
{
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    [Tooltip("starting offset in terms of adding to elapsedTime, startingOffset/frequency = amount of change, i think")]
    [SerializeField] private float startingOffset;
    private float startingY;
    private float elapsedTime;

    private void Start()
    {
        startingY = transform.position.y;
        elapsedTime += startingOffset / frequency;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        Vector3 newPos = transform.position;
        newPos.y = startingY + (amplitude * Mathf.Sin(elapsedTime / frequency));
        transform.position = newPos;
    }
}
