using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerAimer : MonoBehaviour
{
    public TowerController towerController;
    //public Transform currentCanvas;

    //Swipe variables
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;

    [SerializeField]
    private float swipeResistance = 5f;


  
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("Cube"))
        {
            towerController.FollowTarget.position = other.transform.position + Vector3.up*0.6f;

            Debug.Log(other.transform.name);
            other.transform.GetChild(1).gameObject.SetActive(true);

            if(towerController.currentCanvas && other.transform.GetChild(1) != towerController.currentCanvas)
            {
                towerController.currentCanvas.gameObject.SetActive(false);
            }
            towerController.currentCanvas = other.transform.GetChild(1);


            //vcam.m_Follow = other.transform;
        }
    }
    
}
