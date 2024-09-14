using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    PlayerMovement Player;
    public float obstacleLength;
    public static Obstacle[] obstacles;
    public float halfHeight;
    public BoxCollider2D boxCollider;

    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerMovement>();   
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            // Get the size of the BoxCollider2D
            Vector2 size = boxCollider.size;

            // Calculate the half height of the BoxCollider2D
            halfHeight = size.y * 0.5f;
        }
        else
        {
            Debug.LogError("BoxCollider2D component not found!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        pos.x -= Player.Velocity.x * Time.fixedDeltaTime;
        if (pos.x < -100)
        {
            Destroy(gameObject);
        }

        transform.position = pos;
    }

    public float GetObstacleLength()
    {
        return obstacleLength;
    }
}
