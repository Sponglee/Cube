using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : Singleton<FadeCanvas>
{


    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 3.0f; //Minimum logo time;

    public bool FadeInBool = true;
    public float fadeInDuration = 1f;


    private void Start()
    {
        fadeGroup = FindObjectOfType<CanvasGroup>();

        if(FadeInBool)
        {
            fadeGroup.alpha = 1;

            //Get a timestep of a completion time
            if (Time.timeSinceLevelLoad < minimumLogoTime)
                loadTime = minimumLogoTime;
            else
                loadTime = Time.time;
        }
        else
            fadeGroup.alpha = 0;
           


       

    }


    private void Update()
    {
        if(FadeInBool)
        {
            //FadeIn
            if (Time.timeSinceLevelLoad < minimumLogoTime)
                fadeGroup.alpha = 1 - Time.timeSinceLevelLoad/fadeInDuration;

        }

    }

    ////FadeOut
    public void FadeOut(float time, Color color)
    {
        fadeGroup.transform.GetComponent<Image>().color = color;
        StartCoroutine(StopFadeOut(time));

    }
    
    private IEnumerator StopFadeOut(float duration)
    {
        float tmpTime = 0;
        while(tmpTime<duration)
        {
            tmpTime += Time.deltaTime; 
            fadeGroup.alpha = tmpTime/duration;
            yield return null;
        }
    }
}