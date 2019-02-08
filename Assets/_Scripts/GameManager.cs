using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //Floor material
    public Material bottomMaterial;



    //Cinemachine camera holder
    public Transform camHolder;
    //Reference point for camera
    public Transform currentCamPoints;
    //Physical Camera ref for rays etc
    public Camera physicalCam;
    //active Camera position
    public int activeCameraPoint = 0;
    //Active cube ref
    public CubeController activeCube;

    //Character reference
    public CharacterController character;

    //Collor of selected combo
    public int selectedColor = -1;
    public int comboCount = 0;

    //List of clicked elem of same color 
    public List<Transform> comboBuffer;

    //Camera rotation speed
    public float cameraSpeed = 1f;
    //Character movement speed
    public float charInputSpeed = 4f;


    // Start is called before the first frame update
    void Start()
    {
        //Debug Initialize camera
        ChangeCameraState(activeCube.cameraPoints[activeCameraPoint], activeCube.transform);
        
        //Initialize combo buffer
        comboBuffer = new List<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
       

        if(Input.GetMouseButtonDown(0))
        {
            //Click on the bottom cube to select
            GameObject tmpObj = GrabRayObj();
            //Check if clicked a tile and it's bottom
            if (tmpObj.CompareTag("Tile") && activeCube.CubeOpened)
            {
                //Movecharacter to tile when cube opened
                character.Destination = tmpObj.transform.position + new Vector3(0, 0.144f, 0);
            }
            else if(tmpObj.CompareTag("Door") && activeCube.CubeOpened)
            {
                //Move character to a door
                character.Destination = tmpObj.transform.position;
                //character.transform.position = activeCube.transform.position + new Vector3(0, 0.3f, 0);
                
            }


        }
       
    }

    //Swipe manager check
    public void CharacterSwipeResult()
    {
        //Get char movement direction
        if (SwipeManager.Instance.IsSwiping(SwipeDirection.Up))
        {
            MoveCharacter(0);
            character.InputMove = true;
            //Debug.Log("_Up");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Right))
        {
            MoveCharacter(1);
            character.InputMove = true;
            //Debug.Log("_Right");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Down))
        {
            MoveCharacter(2);
            character.InputMove = true;
            //Debug.Log("_Down");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.Left))
        {
            MoveCharacter(3);
            character.InputMove = true;
            //Debug.Log("_Left");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.UpRight))
        {
            MoveCharacter(4);
            character.InputMove = true;
            //Debug.Log("_Left");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.DownRight))
        {
            MoveCharacter(5);
            character.InputMove = true;
            //Debug.Log("_Left");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.DownLeft))
        {
            MoveCharacter(6);
            character.InputMove = true;
            //Debug.Log("_Left");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.UpLeft))
        {
            MoveCharacter(7);
            character.InputMove = true;
            //Debug.Log("_Left");
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeDirection.None))
        {

            //Debug.Log("NONE");
        }
    }




    //Character movement through input
    public void MoveCharacter(int crossDir)
    {
        Vector3 dir = currentCamPoints.GetChild(activeCameraPoint)
               .transform.position - new Vector3(activeCube.transform.position.x, currentCamPoints
                   .GetChild(activeCameraPoint).transform.position.y, activeCube.transform.position.z);

        
        Vector3 tmpDir;

        //Get direction to form velocity
        switch (crossDir)
        {
            //0 - up
            case 0:
                {
                     tmpDir = -dir * charInputSpeed;
                }
                break;
            //1 - right
            case 1:
                {
                    tmpDir = Vector3.Cross(dir, Vector3.up) * charInputSpeed;
                }
                break;
            //2 - down
            case 2:
                {
                    tmpDir = dir * charInputSpeed;
                }
                break;
            //3 - left
            case 3:
                {
                    tmpDir = Vector3.Cross(dir, Vector3.down) * charInputSpeed;
                }
                break;
            //4 - upRight
            case 4:
                {
                    tmpDir = -dir * charInputSpeed + Vector3.Cross(dir, Vector3.up) * charInputSpeed ;
                }
                break;
            //5 - downRight
            case 5:
                {
                    tmpDir = dir * charInputSpeed + Vector3.Cross(dir, Vector3.up) * charInputSpeed;
                }
                break;
            //6 - downLeft
            case 6:
                {
                    tmpDir = dir * charInputSpeed + Vector3.Cross(dir, Vector3.down) * charInputSpeed;
                }
                break;
            //7 - upLeft
            case 7:
                {
                    tmpDir = -dir * charInputSpeed + Vector3.Cross(dir, Vector3.down) * charInputSpeed;
                }
                break;
            default:
                {
                    tmpDir = Vector3.zero;
                }
                break;
        }

        //Add vertical velocity if jump pressed
        if(character.JumpBool)
        {
            tmpDir += Vector3.up * 2f;
        }

        Debug.DrawLine(character.transform.position, character.transform.position + tmpDir, Color.blue, 5f);
        //Move character
        character.transform.GetComponent<Rigidbody>().velocity = tmpDir;

    }


    //Reset camera transform and set Parent
    public void ChangeCameraState(Transform cameraParent, Transform target)
    {
        
        currentCamPoints = cameraParent.parent;
        camHolder.SetParent(cameraParent);
        camHolder.GetComponent<CinemachineVirtualCamera>().m_LookAt = target;
        camHolder.localRotation = Quaternion.identity;
        camHolder.localPosition = Vector3.zero;
        camHolder.localScale = Vector3.one;
    }


    
    //Camera switching 
    public void MoveCamera(Transform cameraPoints)
    {
        
        activeCameraPoint++;
        if (activeCameraPoint >= cameraPoints.childCount)
        {
            activeCameraPoint = 0;
        }
        camHolder.SetParent(cameraPoints.GetChild(activeCameraPoint));
        StartCoroutine(StopCamera(camHolder));
    }

    public IEnumerator StopCamera(Transform camera)
    {
        //Smoothly move camera to point
        while (camera.transform.localPosition != new Vector3(0f, 0f, 0f))
        {
            camera.localPosition = Vector3.Lerp(camera.transform.localPosition, Vector3.zero, cameraSpeed * Time.deltaTime);
            yield return null;
        }
    }




    //Check if elem is ref to other sides
    public void BottomCheck(Transform tile/*, GameObject tmpObj*/)
    {
        CubeElemController tmpTile = tile.GetComponent<CubeElemController>();
        if (tmpTile.BottomLinks.Count != 0)
        {
           
           

            //First color pick
            if (selectedColor == -1)
            {
                tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[0].transform.GetComponent<Renderer>().material;
                //Remember selection
                selectedColor = tmpTile.BottomLinks[0].ElemMatIndex;
                //Check if there's selected color in bottomLinks to add scorecount
                int bottomColor = FindBottomColor(tmpTile, selectedColor);

                //Remember color count
                comboBuffer.Add(tile);
                //Debug.Log("REE");
                comboCount++;
               
                

                //Check buffer
                if (comboCount >= activeCube.colorCombos[selectedColor].Count)
                {

                    //Enable clear buffer on character collision
                    tile.GetComponent<CubeElemController>().ClearBufferTrigger = true;
                    //ClearBuffer();

                }
            }
            else if(selectedColor != 0)
            {
                //Check if there's selected color in bottomLinks - grab material from it
                int bottomColor = FindBottomColor(tmpTile, selectedColor);
                //Debug.Log(":>: " + bottomColor + " :: " + selectedColor);
               

                if (bottomColor >=0 && tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                {
                    //DEBUG
                    //Debug.Log("~~~~" + tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material + " : " + activeCube.materials[selectedColor]);

                    //Paint elem to selectedColor
                    //tile.GetComponent<Renderer>().material = tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material;
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material;

                    //Remember color count
                    if (!comboBuffer.Contains(tile))
                    {
                        comboBuffer.Add(tile);
                        //Debug.Log("REEE");

                        comboCount++;
                    }
                    //else
                    ////{
                    ////    comboCount++;
                    ////}



                    //Debug.Log(selectedColor);
                    //Check buffer
                    if (comboCount >= activeCube.colorCombos[selectedColor].Count)
                    {
                        //Enable clear buffer on character collision
                        tile.GetComponent<CubeElemController>().ClearBufferTrigger = true;
                        //ClearBuffer();

                    }
                }
                else
                {
                    //Clear selection
                    //Enable clear buffer on character collision
                    //ClearBuffer();

                    //Debug.Log("BC : " + bottomColor);
                    tile.GetComponent<CubeElemController>().ClearBufferTrigger = true;
                }

            }
            else
            {
                //Clear selection
                //Enable clear buffer on character collision
                tile.GetComponent<CubeElemController>().ClearBufferTrigger = true;
                //ClearBuffer();

            }

        }
        else
        {
            //Clear selection
            //Enable clear buffer on character collision
            tile.GetComponent<CubeElemController>().ClearBufferTrigger = true;
            //ClearBuffer();
        }



    }

    //Material change for pizzaz
    public IEnumerator ChangeMat(Material start, Material end, float speed)
    {
        
        while(start.color != end.color)
        {
            start.Lerp(start, end, speed);
            //Debug.Log("+");
            yield return null;
        }
    }
    

    //Clear selected color from buffer
    public void ClearBuffer()
    {
       //Each elem in buffer
        foreach (Transform elem in comboBuffer)
        {
            


            //If combo more than colors - clear color from sides
            if(selectedColor != -1 && comboCount >= activeCube.colorCombos[selectedColor].Count)
            {
                 
                //Set Bottom elem to blank if combo
                StartCoroutine(ChangeMat(elem.GetComponent<Renderer>().material, activeCube.materials[0], 0.05f));


                //indexes for bottomlinks to remove
                List<Transform> indexes = new List<Transform>();

                //Iterate through links per elem in buffer
                foreach (CubeElemController link in elem.GetComponent<CubeElemController>().BottomLinks)
                {
                    

                    //Remember selected color links indexes from the wall
                    if (link.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                    {
                        //set SIDE link to blank if combo
                        StartCoroutine(ChangeMat(link.transform.GetComponent<Renderer>().material, activeCube.materials[0], 0.04f));
                       
                        //Remember indexes of bottomLinks to remove
                        indexes.Add(link.transform);
                        //Debug.Log(">>" + indexes.Count);
                        

                    }

                }
               

                //Delete all links from this buffer elem
                foreach (Transform item in indexes)
                {
                    //Debug.Log("INDEX : " + item + ":::: " + elem.GetSiblingIndex());
                    elem.GetComponent<CubeElemController>().BottomLinks.Remove(item.GetComponent<CubeElemController>());
                }
                indexes.Clear();
            }
            else
            {
                //Set Bottom elem to blank if no combo
                StartCoroutine(ChangeMat(elem.GetComponent<Renderer>().material, activeCube.materials[0], 0.08f));

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

        //Check if cube is clear
        CheckCubeEnd();
       
    }
        
    
    //Check if cube is finished
    public void CheckCubeEnd()
    {
        //Debug
        //Debug.Log("_____________________");

        int nullCount = 0;
        for (int i = 0; i < activeCube.colorCombos.Length; i++)
        {

            if (activeCube.colorCombos[i] != null)
            {
                if (activeCube.colorCombos[i].Count == 0)
                    nullCount++;
                //Debug.Log(activeCube.colorCombos[i].Count + " : " + activeCube.materials[i]);
            }
            else
            {
                nullCount++;
                //Debug.Log("NULL");
            }
        }
        //Debug.Log("_____________________");

        //Cube Open condition
        if (nullCount >= activeCube.colorCombos.Length)
        {
            activeCube.anim.SetTrigger("Open");
            activeCube.CubeOpened = true;
            //ChangeCameraState(character.transform.GetChild(0).GetChild(activeCameraPoint), character.transform.GetChild(1));

        }
    }

    
    //Find color from bottomLinks
    public int FindBottomColor(CubeElemController tile, int color)
    {
        int result = -1;

        
            //check material of each link
            for (int i = 0; i < tile.BottomLinks.Count; i++)
            {
                //Check for same color in links
                if (tile.BottomLinks[i].ElemMatIndex == color)
                {
                    //if it's first index
                    if (result == -1)
                    {
                        result = i;

                    }
                    //If second - add up to comboCount
                    else
                    {
                        //If this tile wasn't checked yet
                        if (!comboBuffer.Contains(tile.transform))
                        {
                            Debug.Log("DOUBLE LINK");
                            comboCount++;
                        }
                    }
                }
            }
            //Debug.Log("RESULT :  " + result);
        
        
        return result;
    }

   

    //Elem pick
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
