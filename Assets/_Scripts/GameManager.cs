using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Material bottomMaterial;

    //Cinemachine camera holder
    public Transform camHolder;
    //Physical Camera ref for rays etc
    public Camera physicalCam;
    //Active cube ref
    public CubeController activeCube;


    //Collor of selected combo
    public int selectedColor = -1;
    public int comboCount = 0;

    //List of clicked elem of same color 
    public List<Transform> comboBuffer;


    // Start is called before the first frame update
    void Start()
    {
        //Debug Initialize camera
        ChangeCameraState(activeCube.cameraPoints[activeCube.activeCameraPoint]);
        //Initialize combo buffer
        comboBuffer = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Click on the bottom cube to select
            GameObject tmpObj = GrabRayObj();
            if (tmpObj.CompareTag("Tile"))
            {
                BottomCheck(tmpObj.transform);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Switch between camera points
            activeCube.MoveCamera(camHolder);
        }
        else if(Input.GetMouseButtonDown(2))
        {
            //Debug open cube anim trigger
            GameObject tmpObj = GrabRayObj();
            if (tmpObj.CompareTag("Tile"))
            {
                tmpObj.transform.parent.parent.parent.parent.GetComponent<Animator>().SetTrigger("Open");
            }
        }
    }

   
    //Check if elem is ref to other sides
    public void BottomCheck(Transform tile/*, GameObject tmpObj*/)
    {
        CubeElemController tmpTile = tile.GetComponent<CubeElemController>();
        if (tmpTile.BottomLinks.Count != 0)
        {
            //Check if there's selected color in bottomLinks - grab material from it
            int bottomColor = FindBottomColor(tmpTile, selectedColor);
           

            //First color pick
            if (selectedColor == -1)
            {
                tile.GetComponent<Renderer>().material = tmpTile.BottomLinks[0].transform.GetComponent<Renderer>().material;
                //Remember selection
                selectedColor = tmpTile.BottomLinks[0].ElemMatIndex;
                //Remember color count
                comboBuffer.Add(tile);
                comboCount++;
            }
            else if(selectedColor != 0)
            {
                Debug.Log(":>: " + bottomColor + " :: " + selectedColor);
                //DEBUG
                Debug.Log("~~~~" + tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material + " : " + activeCube.materials[selectedColor]);

                if (tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                {

                    Debug.Log("ENTER");
                    //Paint elem to selectedColor
                    tile.GetComponent<Renderer>().material = tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material;

                    //Remember color count
                    if (!comboBuffer.Contains(tile))
                    {
                        comboBuffer.Add(tile);
                        comboCount++;
                    }
                    //else
                    ////{
                    ////    comboCount++;
                    ////}



                    Debug.Log(selectedColor);
                    //ComboClear condition
                    if (comboCount >= activeCube.colorCombos[selectedColor].Count)
                    {
                        Debug.Log("!!!!!!!!!!!!!!!!!1GAMEOVER!!!!!!!!!!!!!!!!!!");

                        ClearBuffer();
                    }
                }
                else
                {
                    ClearBuffer();
                }
                
            }
            else
            {
                
                ClearBuffer();
            }
           
        }
        else
        {
            ClearBuffer();
        }



    }

    //Clear selected color from buffer
    public void ClearBuffer()
    {
       
        foreach (Transform elem in comboBuffer)
        {
            //Set elem to blank
            elem.GetComponent<Renderer>().material = activeCube.materials[0];



            //If combo more than colors - clear color from sides
            if(selectedColor != -1 && comboCount >= activeCube.colorCombos[selectedColor].Count)
            {
                //For link removal
                List<int> indexes = new List<int>();

                foreach (CubeElemController link in elem.GetComponent<CubeElemController>().BottomLinks)
                {
                    //DElete selected color ones from the wall
                    if (link.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                    {
                        link.transform.GetComponent<Renderer>().material = activeCube.materials[0];
                        indexes.Add(elem.GetComponent<CubeElemController>().BottomLinks.IndexOf(link));

                    }
                }

                //Delete links on bottom elems
                for (int i = 0; i < indexes.Count; i++)
                {
                    elem.GetComponent<CubeElemController>().BottomLinks.RemoveAt(indexes[i]);
                }

            }
        }
            

        //Delete selected colors from links
        if (selectedColor != -1 && comboCount >= activeCube.colorCombos[selectedColor].Count)
        {
            activeCube.colorCombos[selectedColor].Clear();


        }

        selectedColor = -1;
        comboCount = 0;
        comboBuffer.Clear();


        //Debug
        Debug.Log("_____________________");

        for (int i = 0; i < activeCube.colorCombos.Length; i++)
        {
            if (activeCube.colorCombos[i] != null)
                Debug.Log(activeCube.colorCombos[i].Count + " : " + activeCube.materials[i]);
            else
                Debug.Log("NULL");
        }
        Debug.Log("_____________________");
    }
                

    //Find color from bottomLinks
    public int FindBottomColor(CubeElemController tile, int color)
    {
        int result = -1;

        for (int i = 0; i < tile.BottomLinks.Count; i++)
        {
            if (tile.BottomLinks[i].ElemMatIndex == color)
            {
                //if it's first index
                if (result == -1)
                    result = i;
                //If second - add up to comboCount
                else
                    comboCount++;
            }
        }
        return result;
    }

    //Reset camera transform and set Parent
    public void ChangeCameraState(Transform cameraParent)
    {
        
        camHolder.SetParent(cameraParent);
        camHolder.GetComponent<CinemachineVirtualCamera>().m_LookAt = activeCube.transform;
        camHolder.localRotation = Quaternion.identity;
        camHolder.localPosition = Vector3.zero;
        camHolder.localScale = Vector3.one;
    }

    public GameObject GrabRayObj()
    {
        RaycastHit hit;
        Ray ray = physicalCam.ScreenPointToRay(Input.mousePosition);

        Debug.DrawLine(camHolder.transform.position, ray.direction, Color.red, 5f);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHit = hit.transform.gameObject;
            return objectHit;

        }
        return null;
    }

}
