using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeElemController : MonoBehaviour
{
    //Reference to cubeController
    public CubeController cubeController;
    //Reference to side
    [SerializeField]
    private string sideName;
    //Index of elem
    [SerializeField]
    private int elemIndex;
    //Coresponding bottom index
    private int bottomRef;


   


    // Start is called before the first frame update
    void Start()
    {
        
        //Grab a reference to controller
        cubeController = transform.parent.parent.parent.parent.GetComponent<CubeController>();


        //Get index and side name for this elem
        elemIndex = transform.GetSiblingIndex();
        sideName = transform.parent.parent.parent.name;




        //if (sideName == "CubeBottom")
        //{
           
        //}
        //else if (sideName == "CubeFront")
        //{
        //    Debug.Log(sideName);
        //}
        //else if (sideName == "CubeRight")
        //{
        //    Debug.Log(sideName);
        //}
        //else if (sideName == "CubeBack")
        //{
        //    Debug.Log(sideName);
        //}
        //else if (sideName == "CubeLeft")
        //{
        //    Debug.Log(sideName);
        //}

    }

    


   
}
