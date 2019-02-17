using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAimer : MonoBehaviour
{

    public CinemachineVirtualCamera vcam;
    public Transform FollowTarget;



    private void Update()
    {
        
        if(Input.touchCount>0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector3 tmpPos = new Vector3(transform.position.x, FollowTarget.position.y, transform.position.z);
               
                StartCoroutine(StopLook(tmpPos,0.2f));
            }
        }
    }



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

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.CompareTag("Cube"))
        {
            FollowTarget.position = other.transform.position + Vector3.up*0.6f;
            //vcam.m_Follow = other.transform;
        }
    }
    //private void OnTriggerExit(Collider other)
    //{

    //    if (other.gameObject.CompareTag("Cube"))
    //    {
    //        FollowTarget.position = new Vector3(FollowTarget.position.x, transform.position.y, FollowTarget.position.z);
    //    }
    //}
}
