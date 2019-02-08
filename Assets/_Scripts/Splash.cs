using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash: MonoBehaviour
{

    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 3.0f; //Minimum logo time;


    private void Start()
    {

        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1;


        //Get a timestep of a completion time
        if (Time.time < minimumLogoTime)
            loadTime = minimumLogoTime;
        else
            loadTime = Time.time;




    }


    private void Update()
    {
        //FadeIn
        if (Time.time < minimumLogoTime)
            fadeGroup.alpha = 1 - Time.time;
        //FadeOut
        if (Time.time > minimumLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minimumLogoTime;
            if (fadeGroup.alpha >= 1)
            {
                if(PlayerPrefs.GetInt("TutorialStep",0) == 0)
                {

                    SceneManager.LoadScene("Tutorial");
                }
                else
                {
                    SceneManager.LoadScene("Main");
                }
            }
        }



    }
}