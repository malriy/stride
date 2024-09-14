using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    PlayerMovement player;

    public AudioSource jumpAudio;
    public AudioSource landingAudio;

    public AudioClip jumpSound;
    public AudioClip landSound;

    private AudioSource[] allAudio;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        jumpAudio.clip = jumpSound;
        landingAudio.clip = landSound;

        allAudio = FindObjectsOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead)
        {
            FinishGame();
        }
    }

    public void PlayJumpSound()
    {
        if (!player.isDead)
        {
            jumpAudio.Play();
        }
    }

    public void PlayLandingSound()
    {
        if (!player.isDead)
        {
            landingAudio.Play();
        }
    }

    void FinishGame()
    {
        // Stop all audio sources
        foreach (AudioSource audioSource in allAudio)
        {
            audioSource.Stop();
        }
    }
}
