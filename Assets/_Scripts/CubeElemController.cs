﻿using System;
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
    public string SideName { get => sideName; set => sideName = value; }


    //Element's material index in materials array
    [SerializeField]
    private int elemMatIndex;
    public int ElemMatIndex { get => elemMatIndex; set => elemMatIndex = value; }
    //Element's material
    [SerializeField]
    private Material elemMat;
    public Material ElemMat { get => elemMat; set => elemMat = value; }
    //elemCount size
    private int sideLength;
    //Index of elem
    [SerializeField]
    private int elemIndex;
    public int ElemIndex { get => elemIndex; set => elemIndex = value; }
  
    //Coresponding bottom index
    [SerializeField]
    private int bottomRef;
    public int BottomRef { get => bottomRef; set => bottomRef = value; }
    
    //Bottom references for checking
    [SerializeField]
    private List<CubeElemController> bottomLinks;
    public List<CubeElemController> BottomLinks { get => bottomLinks; set => bottomLinks = value; }
  
    private void Awake()
    {
        if (SideName == "CubeBottom")
        {
            bottomLinks = new List<CubeElemController>();
        }
        //Grab a reference to controller
        cubeController = transform.parent.parent.parent.parent.GetComponent<CubeController>();
        //Get index and side name for this elem
        ElemIndex = transform.GetSiblingIndex();
        SideName = transform.parent.parent.parent.name;
        sideLength = cubeController.elemCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Delay bottom references 
        StartCoroutine(StopElemStart());
       
    }

    private IEnumerator StopElemStart()
    {
        yield return new WaitForSecondsRealtime(0.2f); 
        //Debug.Log(sideName);
        //Grab references to coresponding bottom elems
        int i;
        int j;

        //Debug.Log(cubeController.sideMatrices.Count);
        if (SideName == "CubeBottom")
        {
            BottomRef = ElemIndex;
        }
        else if (SideName == "CubeFront")
        {
            i = ElemIndex / sideLength;
            j = ElemIndex % sideLength;
            BottomRef = cubeController.sideMatrices[1][i, j];
            BottomAdd();
        }
        else if (SideName == "CubeRight")
        {
            i = ElemIndex / sideLength;
            j = ElemIndex % sideLength;
            BottomRef = cubeController.sideMatrices[2][i, j];
            BottomAdd();
        }
        else if (SideName == "CubeBack")
        {
            i = ElemIndex / sideLength;
            j = ElemIndex % sideLength;
            BottomRef = cubeController.sideMatrices[3][i, j];
            BottomAdd();
        }
        else if (SideName == "CubeLeft")
        {
            i = ElemIndex / sideLength;
            j = ElemIndex % sideLength;
            BottomRef = cubeController.sideMatrices[4][i, j];
            BottomAdd();
        }
    }
    

    //Bottom links
    public void BottomAdd()
    {
        //Debug.Log(transform.GetComponent<Renderer>().material.color + "::::::" + cubeController.materials[0].color);
        if (transform.GetComponent<Renderer>().material.color != cubeController.materials[0].color)
        {
            cubeController.cubeBottom.GetChild(0).GetChild(0).GetChild(bottomRef).GetComponent<CubeElemController>().BottomLinks.Add(this);

        }

    }

    //Selected material for character to paint tiles with
    public Material materialInteraction;
    public bool ClearBufferTrigger = false;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            if(materialInteraction != null)
            {
                //Set Material
                transform.GetComponent<Renderer>().material = materialInteraction;
                //Reset selected material
                materialInteraction = null;
            }
            if(ClearBufferTrigger)
            {
                GameManager.Instance.ClearBuffer(transform);
                ClearBufferTrigger = false;
            }
            //GameManager.Instance.BottomCheck(transform);
        }
    }

}
