using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerAimer : MonoBehaviour
{
    public Camera physicCam;
    public CinemachineVirtualCamera vcam;
    public Transform FollowTarget;

    public Transform currentCanvas;

    private void Update()
    {
        
        if(Input.touchCount>0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (currentCanvas && currentCanvas.gameObject.activeSelf)
                {
                    GameObject tmp = GrabRayObj();

                    if (tmp && tmp.CompareTag("Cube"))
                    {
                        StartCoroutine(StopLoadTransition("Main"));
                    }
                }
            }
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector3 tmpPos = new Vector3(transform.position.x, FollowTarget.position.y, transform.position.z);
               
                StartCoroutine(StopLook(tmpPos,0.2f));
            }
        }


    }



    public IEnumerator StopLoadTransition(string scene)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(scene);
    }



    //Elem pick
    public GameObject GrabRayObj()
    {



        Vector3 rayPos = Input.mousePosition;
        rayPos.z = physicCam.farClipPlane;

        rayPos = physicCam.ScreenToWorldPoint(rayPos);





        Debug.DrawLine(rayPos, physicCam.transform.position, Color.red, 5f);

        RaycastHit[] hits = Physics.RaycastAll(physicCam.transform.position, rayPos);

        if (hits.Length != 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Cube"))
                {
                    GameObject objectHit = hit.transform.gameObject;
                    Debug.Log(objectHit.tag);
                    return objectHit;

                }

            }
        }


        return null;
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("Cube"))
        {
            FollowTarget.position = other.transform.position + Vector3.up*0.6f;
            
            other.transform.GetChild(1).gameObject.SetActive(true);

            if(currentCanvas && other.transform.GetChild(1) != currentCanvas)
            {
                currentCanvas.gameObject.SetActive(false);
            }
            currentCanvas = other.transform.GetChild(1);


            //vcam.m_Follow = other.transform;
        }
    }
    //private void OnTriggerExit(Collider other)
    //{

    //    if (other.gameObject.CompareTag("Cube"))
    //    {
    //        other.transform.GetChild(1).gameObject.SetActive(false);
    //    }
    //}



    public IEnumerator StopLook(Vector3 tmpPos, float duration)
    {
        float i = 0f;
        float rate = 1f / duration;
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(transform.position, tmpPos, i);
            yield return null;
        }

    }
}
