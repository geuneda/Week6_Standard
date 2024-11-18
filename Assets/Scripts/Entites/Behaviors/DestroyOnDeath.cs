using System;
using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    private HealthSystem healthSystem;
    private Rigidbody2D rigidBody;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        rigidBody = GetComponent<Rigidbody2D>();
        healthSystem.OnDeath += Ondeath;
    }

    private void Ondeath()
    {
        rigidBody.velocity = Vector2.zero;

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        foreach (Behaviour Behaviour in GetComponentsInChildren<Behaviour>())
        {
            Behaviour.enabled = false;
        }

        Destroy(gameObject, 2f);
    }
}