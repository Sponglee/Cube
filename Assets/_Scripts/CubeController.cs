
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    //Material pool
    public Material[] materials;
    //Active Materials
    //List<Material> tmpMats;

    //Animator reference
    public Animator anim;
    //Sides references
    public Transform cubeBottom;

    //Color combo references
    public List<Transform>[] colorCombos;

    public Transform[] cubeSides;

    //Elem count per side in a row
    public int elemCount;
    //Side matrix
    public int[,] initialMatrix;
    int[,] tmpMatrix;
    public List<int[,]> sideMatrices;
    
    
    //Camera reference points
    public Transform[] cameraPoints;

    //Obstacle prefab
    public GameObject obstaclePref;

    //Cube opened bool
    private bool cubeOpened = false;
    public bool CubeOpened { get => cubeOpened; set => cubeOpened = value; }

    //Tutorial toggle
    public bool TutorialCube = false;
    //Last cube on the level toggle
    public bool EndCube = false;

    private void Awake()
    {
        //Initialize animator
        anim = GetComponent<Animator>();
        //Initialize matrix for sides
        initialMatrix = new int[elemCount, elemCount];
        //Initialize list of sides for elems
        sideMatrices = new List<int[,]>();
        //FILL INDEX MATRIX
        int index = 0;

        for (int i = 0; i < elemCount; i++)
        {
            for (int j = 0; j < elemCount; j++)
            {
                initialMatrix[i, j] = index;
                index++;
            }

        }

        //Get rotated matrices for each side
        //rotation iterator;
        int degrees = -90;
        for (int i = 0; i < 5; i++)
        {
            //Debug.Log(degrees);
            switch (degrees)
            {
                //BottomSide
                case -90:
                    {
                        sideMatrices.Add(initialMatrix);
                    }
                    break;
                //FrontSide
                case 0:
                    {
                        sideMatrices.Add(RevertRows(initialMatrix));
                    }
                    break;
                //RightSide
                case 90:
                    {
                        sideMatrices.Add(TransposeMatrix(initialMatrix));
                    }
                    break;
                //BackSide
                case 180:
                    {
                        sideMatrices.Add(RevertColumns(initialMatrix));
                    }
                    break;
                //LeftSide
                case 270:
                    {
                        sideMatrices.Add(RevertColumns(TransposeMatrix(RevertColumns(initialMatrix))));
                    }
                    break;
                default:
                    break;
            }
            degrees += 90;
        }

    }

    
    public CubeData cubeInfo;

    // Start is called before the first frame update
    void Start()
    {

        //EndCubeToggle
        //if(EndCube)
        //{
        //    Instantiate(GameManager.Instance.elevator, transform.position, Quaternion.identity);
        //}
        //Initialize tmpMats
        //tmpMats = new List<Material>();
        //Initialize colorCombos
        colorCombos = new List<Transform>[materials.Length];

        //Initialize cube sides if not tutorial
        if (!TutorialCube)
        {
            //RandomizeCube();

            LoadCubeData();
        }
        //#region TUTORIAL
        //else if(TutorialCube)
        //{
            
        //    PlayerPrefs.SetInt("TutorialStep", 1);
           

        //    ////Red
        //    //colorCombos[1] = new List<Transform>();
        //    ////Green
        //    //colorCombos[2] = new List<Transform>();

        //    foreach (Transform cubeSide in cubeSides)
        //    {

        //        //Set and remember random color per side
        //        int randomMat = 0;

        //        if (cubeSide.name == "CubeFront")
        //        {
        //            randomMat = 1;
        //        }
        //        else if(cubeSide.name == "CubeBack")
        //        {
        //            randomMat = 2;
        //        }

        //        Material tmpMat = materials[randomMat];
        //        //tmpMats.Add(tmpMat);

        //        //Debug.Log(":::" + randomMat);
        //        //Initialize color combos lists to anything except first material 
        //        if (randomMat != 0 && colorCombos[randomMat] == null)
        //            colorCombos[randomMat] = new List<Transform>();

        //        //Grab references to each element per side
        //        foreach (Transform child in cubeSide.GetChild(0).GetChild(0))
        //        {
                  
        //            if (child.GetComponent<Renderer>().material.color != materials[0].color)
        //            {
        //                //Side color elem
        //                child.GetComponent<Renderer>().material = tmpMat;
        //                CubeElemController tmpController = child.GetComponent<CubeElemController>();
        //                tmpController.ElemMat = tmpMat;
        //                tmpController.ElemMatIndex = randomMat;

        //                //Add elem to combo per color
        //                colorCombos[randomMat].Add(child);
        //                //Debug.Log(colorCombos[randomMat].Count);

        //            }
        //        }
        //    }
           

        //}
        //#endregion
    }
   

 

    public void LoadCubeData()
    {
        CubeData loadCubeInfo = SaveSystem.LoadLevel(ProgressManager.Instance.towerIndex).cubes[ProgressManager.Instance.CurrentCube];

        for (int i = 0; i < cubeSides.Length; i++)
        {
            //Get and remember random color per side
            int randomMat = loadCubeInfo.sides[i].sideMat;


            //Initialize color combos lists to anything except first material 
            if (randomMat != 0 && colorCombos[randomMat] == null)
                colorCombos[randomMat] = new List<Transform>();


            //Grab references to each element per side
            foreach (Transform child in cubeSides[i].GetChild(0).GetChild(0))
            {

                Material tmpMat = materials[loadCubeInfo.sides[i].elemColors[child.GetSiblingIndex()]];

                //Side color elem
                child.GetComponent<Renderer>().material = tmpMat;
                CubeElemController tmpController = child.GetComponent<CubeElemController>();
                tmpController.ElemMat = tmpMat;
                tmpController.ElemMatIndex = randomMat;

                //Add combo if not 0
               if(loadCubeInfo.sides[i].elemColors[child.GetSiblingIndex()] != 0)
               {
                    //Add elem to combo per color
                    colorCombos[randomMat].Add(child);
               }

            }

            Debug.Log("SIDE " + cubeInfo.sides[i]+ ": " + colorCombos[randomMat].Count);
        }
       
    }


    //Check if there's already same bottom ref for this color
    public bool CheckForDoubles(Transform child, int mat, CubeElemController tmpCont)
    {
        
        foreach (Transform tmpCombo in colorCombos[mat])
        {
            //If checked elem has same bottomRef - dont add;
            if (tmpCombo.GetComponent<CubeElemController>().BottomRef == child.GetComponent<CubeElemController>().BottomRef 
                /*&& tmpCombo.GetComponent<Renderer>().material.color == */)
            {
                Debug.Log("REEE SAME " + child.GetComponent<CubeElemController>().ElemIndex +
                    " : " + child.GetComponent<Renderer>().material+" : " + child.GetComponent<CubeElemController>().BottomRef);
                return true;
            }
        }
        return false;
    }


     //Find color from bottomLinks
    public int FindBottomColor(CubeElemController tile, int color)
    {
        for (int i = 0; i < tile.BottomLinks.Count; i++)
        {
            if (tile.BottomLinks[i].ElemMatIndex == color)
            {
                Debug.Log(i);
                return i;
            }
        }
        return 0;
    }
   



      
    //MATRIX CALCULATIONS

    
    //Transpose matrix
    public int[,] TransposeMatrix(int[,] a)
    { 
        int[,] result = new int[elemCount, elemCount];
       
        for (int i = 0; i < elemCount; i++)
        {
            for (int j = 0; j < elemCount; j++)
            {
                result[j, i] = a[i, j];
            }
        }
        return result;
    }

    //Revert rows
    public int[,] RevertColumns(int[,] a)
    {

        int[,] result = new int[elemCount, elemCount]; 

        for (int i = 0; i < elemCount; i++)
        {
            int[] tmpArray = new int[elemCount];

            for (int j = 0; j < elemCount; j++)
            {
                tmpArray[j] = a[i,j];
            }

            Array.Reverse(tmpArray);

            for (int j = 0; j < elemCount; j++)
            {
                result[i, j] = tmpArray[j];
                //Debug.Log(result[i, j]);
            }
        }
        return result;
    }

    //Revert rows
    public int[,] RevertRows(int[,] a)
    {

        int[,] result = new int[elemCount, elemCount];

        for (int i = 0; i < elemCount; i++)
        {
            int[] tmpArray = new int[elemCount];

            for (int j = 0; j < elemCount; j++)
            {
                result[elemCount - 1 - i, j] = a[i,j];
            }
        }
        return result;
    }
}
