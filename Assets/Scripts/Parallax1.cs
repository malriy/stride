using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax1 : MonoBehaviour
{
    public float depth = 1;

    PlayerMovement Player;

    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float realVelocity = Player.Velocity.x / depth;
        Vector2 pos = transform.position;

        pos.x -= realVelocity * Time.fixedDeltaTime;

        if (pos.x <= -17)
            pos.x = 47;

        if (!Player.isDead)
        {
            transform.position = pos;
        }
    }
}
