
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CubeController : MonoBehaviour
{

    //Material pool
    public Material[] materials;
    //Active Materials
    List<Material> tmpMats;

    //Sides references
    public Transform cubeBottom;

    //Color combo references
    public List<Transform>[] colorCombos;

    public Transform[] cubeSides;

    //Elem count per side in a row
    public int elemCount = 3;
    //Side matrix
    public int[,] initialMatrix;
    int[,] tmpMatrix;
    public List<int[,]> sideMatrices;
    //Camera reference points
    public Transform[] cameraPoints;
    public int activeCameraPoint = 0;


    private void Awake()
    {
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


 

    // Start is called before the first frame update
    void Start()
    {
        //Initialize tmpMats
        tmpMats = new List<Material>();
        //Initialize colorCombos
        colorCombos = new List<Transform>[materials.Length];

        //Initialize cube sides
        foreach (Transform cubeSide in cubeSides)
        {
            //Set and remember random color per side
            int randomMat = UnityEngine.Random.Range(1, materials.Length);
            Material tmpMat = materials[randomMat];
            tmpMats.Add(tmpMat);

            //Debug.Log(":::" + randomMat);
            //Initialize color combos lists to anything except first material 
            if (randomMat != 0 && colorCombos[randomMat] == null)
                colorCombos[randomMat] = new List<Transform>();

            //Grab references to each element per side
            foreach (Transform child in cubeSide.GetChild(0).GetChild(0))
            {
                //Randomize side's elem colorss
                float matRandomizer = UnityEngine.Random.Range(0, 100);
                if (matRandomizer <= 50)
                {
                    //Side color elem
                    child.GetComponent<Renderer>().material = tmpMat;
                    CubeElemController tmpController = child.GetComponent<CubeElemController>();
                    tmpController.ElemMat = tmpMat;
                    tmpController.ElemMatIndex = randomMat;

                    //Add elem to combo per color
                    colorCombos[randomMat].Add(child);

                }
                else
                {
                    //Blank elem
                    child.GetComponent<Renderer>().material = materials[0];
                }
            }
        }
        StartCoroutine(StopCheckDoubles());
       
    }


    //Check for all doublicates in colorCombos
    private IEnumerator StopCheckDoubles()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        

        foreach (Transform child in cubeBottom.GetChild(0).GetChild(0))
        {
            List<int> links = new List<int>();
            foreach (CubeElemController item in child.GetComponent<CubeElemController>().BottomLinks)
            {
                
                if (links.Contains(item.ElemMatIndex))
                {
                    links.Add(item.ElemMatIndex);
                    //////////////////////////colorCombos[]
                }
            }
        }


        //Debug


        Debug.Log("_____________________");

        for (int i = 0; i < colorCombos.Length; i++)
        {
            if (colorCombos[i] != null)
                Debug.Log(colorCombos[i].Count + " : " + materials[i]);
            else
                Debug.Log("NULL");
        }
        Debug.Log("_____________________");

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
    //Camera switching 
    public void MoveCamera(Transform camera)
    {
        activeCameraPoint++;
        if(activeCameraPoint>= cameraPoints.Length)
        {
            activeCameraPoint = 0;
        }
        camera.SetParent(cameraPoints[activeCameraPoint]);
        StartCoroutine(StopCamera(camera));
    }

    public float cameraSpeed = 1f;
    public IEnumerator StopCamera(Transform camera)
    {
        //Smoothly move camera to point
        while (camera.transform.localPosition != new Vector3(0f,0f,0f))
        {
            camera.localPosition = Vector3.Lerp(camera.transform.localPosition, Vector3.zero, cameraSpeed * Time.deltaTime);
            yield return null;
        }
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
