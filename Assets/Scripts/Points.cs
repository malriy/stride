using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public float depth = 1.0f;
    public int chipScore;
    private PlayerMovement Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float realVelocity = Player.Velocity.x / depth;
        Vector2 pos = transform.position;

        pos.x -= realVelocity * Time.fixedDeltaTime;

        if (pos.x <= -1)
            Destroy(GameObject.Find("Swipe"));
        if (!Player.isDead)
        {
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player") {
            Player.score += chipScore;
            Destroy(gameObject);
        }
    }
}
