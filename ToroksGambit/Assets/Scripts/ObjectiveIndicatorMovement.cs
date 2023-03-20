using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveIndicatorMovement : MonoBehaviour
{
    [SerializeField] private float newScale;
    [SerializeField] private float scaleLength;
    [SerializeField] private float scaledTime;
    [SerializeField] private float speed = 0.02f;
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        newScale = 0.08f;
        scaleLength = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        scaledTime = Time.time * speed;
        // scale in, scale out
        //if (GameStateManager.instance.GetGameState() == GameStateManager.GameState.deployment)
        {
            transform.localScale = new Vector3(Mathf.PingPong(scaledTime, scaleLength) + newScale, Mathf.PingPong(scaledTime, scaleLength) + newScale, Mathf.PingPong(scaledTime, scaleLength) + newScale);
        }
        //else
        //{
        //    transform.localScale = originalScale;
        //}
    }
}
