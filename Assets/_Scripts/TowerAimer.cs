﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerAimer : MonoBehaviour
{
    public TowerController towerController;
    public LevelsController levelController;

    //public Transform currentCanvas;

    //Swipe variables
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;

    [SerializeField]
    private float swipeResistance = 5f;


  
    private void OnTriggerEnter(Collider other)
    {
      //Tower
       if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (other.gameObject.CompareTag("Cube"))
            {
                towerController.FollowTarget.position = other.transform.position + Vector3.up * 0.6f;

                Debug.Log(other.transform.name);
                other.transform.GetChild(1).gameObject.SetActive(true);

                if (towerController.currentCanvas && other.transform.GetChild(1) != towerController.currentCanvas)
                {
                    towerController.currentCanvas.gameObject.SetActive(false);


                }
                towerController.currentCanvas = other.transform.GetChild(1);


                //vcam.m_Follow = other.transform;
            }
           


        }
       //Levels
        else if(SceneManager.GetActiveScene().buildIndex == 3)
        {
                Debug.Log("POINK " + other.name);
            if (other.gameObject.CompareTag("Tower"))
            {
                //Move Follow Target to new tower 
                levelController.FollowTarget.position = new Vector3(other.transform.position.x, levelController.FollowTarget.position.y, other.transform.position.z);

                //set currentTower to this tower
                ProgressManager.Instance.CurrentTower = other.transform.parent.GetSiblingIndex();

                //Debug.Log(other.transform.name);
                other.transform.parent.GetChild(1).gameObject.SetActive(true);
                other.transform.parent.GetChild(1).GetComponent<Canvas>().worldCamera = levelController.physicCam;

                if (levelController.currentCanvas && other.transform.parent.GetChild(1) != levelController.currentCanvas)
                {
                    levelController.currentCanvas.gameObject.SetActive(false);
                }
                levelController.currentCanvas = other.transform.parent.GetChild(1);


            }
         

        }
    }

    
}
