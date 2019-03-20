﻿using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetRotator : MonoBehaviour
{
    public float scrollSpeed = 200;



    public Transform cameras;
    public Transform vcamClose;
    public Transform vcamMain;

    public Transform activeCam;
    public Camera physicCam;

    

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetMouseButtonUp(0))
        //{
        //    GameObject rayObj = GrabRayObj();
        //    //Debug.Log(rayObj.transform.parent.name);
        //    if (rayObj != null && rayObj.CompareTag("Tower") && activeCam == vcamMain)
        //    {
        //        activeCam = vcamClose;
        //        ChangeCamState(rayObj);
        //    }
        //    else if (rayObj != null && rayObj.CompareTag("Tower") && activeCam == vcamClose)
        //    {
        //        activeCam = vcamMain;
        //        SceneManager.LoadScene("Main");
        //    }
        //    else if ((rayObj == null || !rayObj.CompareTag("Tower")) && activeCam == vcamClose)
        //    {
        //        activeCam = vcamMain;
        //        ChangeCamState();
        //    }

        //}
    }

    public void ChangeCamState(GameObject rayObj = null)
    {

        

        if (vcamClose.gameObject.activeSelf && rayObj != null)
        {
            //activeCam = vcamClose.transform;
            
        }
        else
        {
            vcamClose.transform.gameObject.GetComponent<CinemachineVirtualCamera>().m_LookAt = rayObj.transform.GetChild(1);

            vcamClose.parent = rayObj.transform.GetChild(1);
            vcamClose.transform.localPosition = new Vector3(rayObj.transform.GetChild(0).localPosition.x, rayObj.transform.GetChild(1).localPosition.y, rayObj.transform.GetChild(0).position.z-2);
            //activeCam = vcamMain.transform;
        }


        vcamClose.gameObject.SetActive(!vcamClose.gameObject.activeSelf);
        vcamMain.gameObject.SetActive(!vcamMain.gameObject.activeSelf);
    }



    //Elem pick
    public GameObject GrabRayObj()
    {



        Vector3 rayPos = Input.mousePosition;
        rayPos.z = physicCam.farClipPlane;

        rayPos = physicCam.ScreenToWorldPoint(rayPos);



    

        Debug.DrawLine(rayPos, physicCam.transform.position, Color.red, 5f);

        RaycastHit[] hits = Physics.RaycastAll(physicCam.transform.position, rayPos);
        
            if(hits.Length!= 0)
            {
                foreach (var hit in hits)
                {
                    if(hit.collider.CompareTag("Tower"))
                    {
                        GameObject objectHit = hit.transform.gameObject;
                        Debug.Log(objectHit.tag);
                        return objectHit;

                    }
               
                }               
            }
            
          
        return null;
    }

    public Transform cameraHolder;
    //Rotate planet
    void OnMouseDrag()
    {

        

            float rotX = Input.GetAxis("Mouse X");
            
            if (Input.touchCount > 0)
            {
                rotX = Input.touches[0].deltaPosition.x;
                
            }

            //Debug.Log("REEE " + rotX + " : " + Input.GetAxis("Mouse Y") + " = " + scrollSpeed);

            //if (rotX > rotResistance)
            //{
            //    cameraHolder.transform.Rotate(Vector3.up, rotX, Space.Self);
            //}

            //Scroll camera and elevator
            cameraHolder.position += new Vector3(0, 0, -rotX*scrollSpeed);
            //elevatorHolder.transform.position += new Vector3(0, -rotX / 120f, 0);
            
        
    }
}