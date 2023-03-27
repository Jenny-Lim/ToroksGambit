using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIParticle : MonoBehaviour
{
    private Vector2 velocity;
    private bool moveToTicketCounter = false;
    public float scaleSpeed;
    public float timeAt100Scale = 1;
    private float timeElapsedAt100Scale = 0;

    public void UpdateParticle(float deltaTime, Transform moveTowards)
    {
        UpdateScale(deltaTime);

        if (transform.localScale.x == 1)
        {
            timeElapsedAt100Scale += Time.deltaTime;
            if (timeElapsedAt100Scale >= timeAt100Scale)
            {
                moveToTicketCounter = true;
            }
        }

        if (moveToTicketCounter)
        {
            MovePosition(deltaTime, moveTowards);
        }
    }

    private void UpdateScale(float deltaTime)
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, deltaTime * scaleSpeed);
    }

    private void MovePosition(float deltaTime, Transform moveTowards)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTowards.localPosition, velocity.magnitude * deltaTime);
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector2 newVel)
    {
        velocity = newVel;
    }

    public void UpdateVelocity(float deltaTime, Transform moveTowards)
    {
        velocity = Vector3.RotateTowards(velocity, (Vector2)moveTowards.localPosition, 3.14f, 1f);
    }
}
