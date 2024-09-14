using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControllerTitle : MonoBehaviour
{
    [SerializeField] RectTransform fader;
    [SerializeField] RectTransform tutorialUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play()
    {
        fader.gameObject.SetActive(true);

        LeanTween.move(fader, Vector3.zero, 2.0f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    public void OpenTutorial()
    {
        tutorialUI.gameObject.SetActive(true);

        LeanTween.scale(tutorialUI, Vector3.one, 2.0f).setEase(LeanTweenType.easeOutQuint);
    }

    public void CloseTutorial()
    {
        LeanTween.scale(tutorialUI, Vector3.zero, 2.0f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
        {
            tutorialUI.gameObject.SetActive(false);
        });
    }
}
