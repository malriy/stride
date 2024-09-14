using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuer : MonoBehaviour
{
    public float movementSpeed = 2.0f;
    public float pursuerRight;

    public AudioSource pursuerAudio;
    public Transform playerTransform;
    public Transform pursuerTransform;

    void Start()
    {

    }

    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            pursuerRight = spriteRenderer.bounds.max.x;
        }

        float distance = Vector3.Distance(playerTransform.position, pursuerTransform.position);

        float minDistance = 5f;  
        float maxDistance = 13f; 

        float volume = Mathf.InverseLerp(maxDistance, minDistance, distance);

        // Clamp the volume to be between 0 and 1
        volume = Mathf.Clamp(volume, 0, 0.2f);

        // Set the volume of the AudioSource
        pursuerAudio.volume = volume;
    }
}
