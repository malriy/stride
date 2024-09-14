using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    PlayerMovement player;
    Text distanceText;

    GameObject results;
    Text finalDistanceText;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();

        finalDistanceText = GameObject.Find("FinalDistance").GetComponent<Text>();
        results = GameObject.Find("Results");
        results.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int distance = Mathf.FloorToInt(player.score);
        distanceText.text = "Score: " + distance;

        if (player.isDead)
        {
            results.SetActive(true);
            finalDistanceText.text = "" + distance;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
