using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CutsceneTransition : MonoBehaviour
{
    [SerializeField] RectTransform fader;

    // Start is called before the first frame update
    void Start()
    {
        fader.gameObject.SetActive(true);

        LeanTween.move(fader, new Vector3(-2500, 0, 0), 1.5f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            fader.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
