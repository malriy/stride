using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviLeft : MonoBehaviour
{
    public float endPos;
    public float resetPos;
    public float waitTime;

    PlayerMovement Player;

    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    private IEnumerator wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        float randomY = Random.Range(4.5f, 9.71f);
        transform.position = new Vector2(resetPos, randomY);
    }

    // Update is called once per frame
    void Update()
    {
        float randomDepth = Random.Range(3f, 6f);
        float realVelocity = Player.Velocity.x / randomDepth;
        Vector2 pos = transform.position;

        pos.x -= realVelocity * Time.fixedDeltaTime;

        if (pos.x <= endPos)
        {
            StartCoroutine(wait(waitTime));
        }

        if (!Player.isDead)
        {
            transform.position = pos;
        }
    }
}