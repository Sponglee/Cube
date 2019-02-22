using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FadeCanvas : Singleton<FadeCanvas>
{


    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 3.0f; //Minimum logo time;


    private void Start()
    {
        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1;

        //Get a timestep of a completion time
        if (Time.timeSinceLevelLoad < minimumLogoTime)
            loadTime = minimumLogoTime;
        else
            loadTime = Time.time;

    }


    private void Update()
    {
        //FadeIn
        if (Time.timeSinceLevelLoad < minimumLogoTime)
            fadeGroup.alpha = 1 - Time.timeSinceLevelLoad;
       
    }

    ////FadeOut
    public void FadeOut(float time)
    {

        StartCoroutine(StopFadeOut(time));

    }
    
    private IEnumerator StopFadeOut(float duration)
    {
        float tmpTime = 0;
        while(tmpTime<duration)
        {
            tmpTime += Time.deltaTime; 
            fadeGroup.alpha = tmpTime;
            yield return null;
        }
    }
}