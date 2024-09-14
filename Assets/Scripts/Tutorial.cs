using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public float depth;

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

        if (pos.x <= -779)
            Destroy(GameObject.Find("Tutorial"));

        if (!Player.isDead)
        {
            transform.position = pos;
        }
    }
}
