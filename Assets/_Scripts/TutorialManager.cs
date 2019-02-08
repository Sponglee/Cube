﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager> {

    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject cameraHand;
    public GameObject doorHand;

    [SerializeField]
    private int tutorialActive = -1;

    public int TutorialActive
    { get => tutorialActive;
        set
        {
            tutorialActive = value;
            PlayerPrefs.SetInt("TutorialStep", value);
        }
    }

    // Use this for initialization
    void Start() {

        tutorialActive = 0;

        if(TutorialActive <= 4)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            //Time.timeScale = 0;
            //rightHand.SetActive(true);
            tutorialActive++;
            CloseTut(1);
        }
    }
    private void Update()
    {
        //TutorialActive = PlayerPrefs.GetInt("TutorialStep", 0);
    }

    public void CloseTut(int first = 0)
    {
       //Movement
        if(first == 1)
        {
            rightHand.SetActive(true);
            rightHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandUP");
            
        }
        //Camera
        else if (first == 2)
        {
            rightHand.SetActive(false);


            cameraHand.SetActive(true);
            cameraHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandCLICK");

        }
        //Jump movement
        else if (first == 3)
        {
            cameraHand.SetActive(false);


            leftHand.SetActive(true);
            leftHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandHOLD"); 
            rightHand.SetActive(true);
            rightHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandUP");

        }
        //Door and camera
        else if (first == 4)
        {
           
           
            leftHand.SetActive(false);
            rightHand.SetActive(false);


            cameraHand.SetActive(true);
            cameraHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandCLICK");
            doorHand.SetActive(true);
            doorHand.transform.GetChild(0).GetComponent<Animator>().Play("TutorialHandCLICK");
           

        }
        else
        {
            tutorialActive = 5;
            //thirdStep.SetActive(false);
            Time.timeScale = 1;
            transform.GetChild(0).gameObject.SetActive(false);
        }
   
    }

}
