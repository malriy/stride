using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour
{
    PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        pos.x -= player.Velocity.x * Time.fixedDeltaTime;

        if (pos.x < -200)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = pos;
    }
}
