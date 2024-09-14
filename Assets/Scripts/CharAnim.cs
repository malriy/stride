using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnim : MonoBehaviour
{
    private Animator animator;
    public PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", player.Velocity.x);

        if (player.isGrounded)
        {
            animator.SetBool("grounded", true);
        }
        else
        {
            animator.SetBool("grounded", false);
        }

        if (player.isShort)
        {
            animator.SetBool("short", true);
        }
        else
        {
            animator.SetBool("short", false);
        }

        if (player.isLong)
        {
            animator.SetBool("long", true);
        }
        else
        {
            animator.SetBool ("long", false);
        }

        if (player.isHit)
        {
            animator.SetBool("hit", true);
        } else
        {
            animator.SetBool("hit", false);
        }

        if (player.isSliding)
        {
            animator.SetBool("slide", true);
        } else
        {
            animator.SetBool("slide", false);
        }
    }
}
