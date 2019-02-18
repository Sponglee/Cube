using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScroller : MonoBehaviour
{
    public float rotSpeed = 20;
    public float scrollSpeed = 2;

    public float rotResistance = 5;
    public Transform cameras;


    public TowerData data;


    //Rotate planet
    void OnMouseDrag()
    {

        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * scrollSpeed;
        if (Input.touchCount > 0)
        {
            rotX = Input.touches[0].deltaPosition.x;
            rotY = Input.touches[0].deltaPosition.y;
        }

        //Debug.Log("REEE " + rotX + " : " + Input.GetAxis("Mouse Y") + " = " + scrollSpeed);

        if(rotX>rotResistance)
            cameras.transform.Rotate(Vector3.up, rotX, Space.Self);
        cameras.transform.position +=  new Vector3(0, -rotY/100f, 0);
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            Debug.Log("REEE");
        }
        
    }



}
