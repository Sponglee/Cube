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
    [SerializeField]
    //elemCount size
    private int sideLength;
    //Index of elem
    [SerializeField]
    private int elemIndex;
    //Coresponding bottom index
    [SerializeField]
    private int bottomRef;
  

   


    // Start is called before the first frame update
    void Start()
    {
        //Grab a reference to controller
        cubeController = transform.parent.parent.parent.parent.GetComponent<CubeController>();


        //Get index and side name for this elem
        elemIndex = transform.GetSiblingIndex();
        sideName = transform.parent.parent.parent.name;
        sideLength = cubeController.elemCount;


        //Grab references to coresponding bottom elems
        int i;
        int j;

        //Debug.Log(cubeController.sideMatrices.Count);
        if (sideName == "CubeBottom")
        {
            bottomRef = elemIndex;
        }
        else if (sideName == "CubeFront")
        {
            bottomRef = elemIndex;
        }
        else if (sideName == "CubeRight")
        {
            i = elemIndex / sideLength;
            j = elemIndex % sideLength;
            bottomRef = cubeController.sideMatrices[2][i, j];
        }
        else if (sideName == "CubeBack")
        {
            i = elemIndex / sideLength;
            j = elemIndex % sideLength;
            bottomRef = cubeController.sideMatrices[3][i, j];
        }
        else if (sideName == "CubeLeft")
        {
            i = elemIndex / sideLength;
            j = elemIndex % sideLength;
            bottomRef = cubeController.sideMatrices[4][i, j];
        }

    }

    


   
}
