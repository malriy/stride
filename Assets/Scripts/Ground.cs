using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ground : MonoBehaviour
{
    PlayerMovement player;

    public float groundHeight;
    BoxCollider2D boxCollider;

    public float screenRight;
    public float groundRight;
    public float groundLeft;
    bool didGenerate = false;
    public bool willGenerate = true;
    //Ground goGround;

    public ChipGenerator chipGenerator;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

        boxCollider = GetComponent<BoxCollider2D>();
        groundHeight = transform.position.y + (boxCollider.size.y / 2);
        screenRight = Camera.main.transform.position.x * 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        chipGenerator = FindObjectOfType<ChipGenerator>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        pos.x -= player.Velocity.x * Time.fixedDeltaTime;

        groundRight = transform.position.x + (boxCollider.size.x / 2);
        groundLeft = transform.position.x - (boxCollider.size.x / 2);

        if (groundRight < 0)
        {
            Destroy(gameObject);
            return;
        }

        if (!didGenerate) 
        {
            if (groundRight < screenRight && willGenerate)
            {
                didGenerate = true;
                generateGround();
            }
        }

        transform.position = pos;
    }

    void generateGround()
    {
        GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;

        float maxY = (groundHeight + 1.8f) - goCollider.size.y / 2;
        float minY = -4.5f;
        float actualY = Random.Range(minY, maxY);

        pos.y = actualY;
        if (pos.y > -1)
            pos.y = Random.Range(minY, -3.5f);

        float xDistance = Random.Range(12.5f, 12.8f);
        pos.x = screenRight + xDistance;
        go.transform.position = pos;

        Ground goGround = go.GetComponent<Ground>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);
        Obstacle.obstacles = Resources.LoadAll<Obstacle>("Obstacles");

        //Obstacle Generation
        int numObstacle = Random.Range(1, 6); //How many obstacles
        List<Vector2> availablePositions = new List<Vector2>();

        float minDistance = 2f;

        float spacing = 16f;
        float halfSpacing = spacing / 2f;

        float halfWidth = goCollider.size.x / 2 - 2f; // Adjusted to avoid spawning at the edges
        float left = go.transform.position.x - halfWidth;
        float right = go.transform.position.x + halfWidth;

        for (float x = left + 4.0f; x < right; x += 8.5f)
        {
            availablePositions.Add(new Vector2(x, goGround.groundHeight));
        }
        Debug.Log($"Available: {availablePositions.Count}");
        //foreach (Vector2 i in availablePositions)
        //{
        //    Debug.Log($"Coord: {i}");
        //}

        for (int i = 0; i < numObstacle && availablePositions.Count > 0; i++)
        {
            int whichItem = Random.Range(0, 3);

            // Randomly select a position from the available positions
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2 boxPos = availablePositions[randomIndex];
   
            float obstacleLength = Obstacle.obstacles[whichItem].obstacleLength;
            float obsHalf = Obstacle.obstacles[whichItem].halfHeight;
            //Debug.Log("obs: " + Obstacle.obstacles[whichItem].name + "(" + boxPos.x + "," + boxPos.y + ") + " + obsHalf);
            boxPos.y += obsHalf;
            bool overlap = Physics2D.OverlapBox(boxPos, new Vector2(obstacleLength / 2f, 0.5f), 0, LayerMask.GetMask("Obstacles")); // Making sure no overlap of obstacles

            if (!overlap)
            {
                GameObject box = Instantiate(Obstacle.obstacles[whichItem].gameObject);
                box.transform.position = boxPos;

                // Remove the selected position and its surroundings from the list
                availablePositions.RemoveAt(randomIndex);
                for (float x = boxPos.x - obstacleLength; x <= boxPos.x + obstacleLength; x += minDistance)
                {
                    availablePositions.RemoveAll(pos => Mathf.Approximately(pos.x, x));
                }

                // Spawn chip
                //chipGenerator.SpawnChip(boxPos);
            }
        }
    }
}
