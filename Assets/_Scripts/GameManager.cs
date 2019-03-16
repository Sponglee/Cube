using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //Floor material
    public Material bottomMaterial;

    //elevator
    public GameObject elevator;
    //elevator Camera ref
    public CinemachineVirtualCamera elVcam;
   
   
    
  

    //Cube prefab
    public GameObject cubePref;

    //Reference point for camera
    public Transform camPointHolder;
    //Active cube ref
    public CubeController activeCube;

    //active Camera position
    [SerializeField]
    private int activeCameraPoint = 0;
    public int ActiveCameraPoint
    {
        get => activeCameraPoint;
        set
        {
            if (value >= camPointHolder.childCount)
            {
                activeCameraPoint = 0;
            }
            else if(value < 0)
            {
                activeCameraPoint = camPointHolder.childCount-1;
            }
            else
            {
                activeCameraPoint = value;
            }
        }
    }



    //Character reference
    public CharacterController character;
    //Physical Camera ref for rays etc
    public Camera physicalCam;
    //Cinemachine camera holder
    public Transform camHolder;


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

        //Instantiate a cube prefab
        if(SceneManager.GetActiveScene().name == "Main")
        {
            GameObject cubeSpawn = Instantiate(cubePref, Vector3.zero, Quaternion.identity);
            activeCube = cubeSpawn.GetComponent<CubeController>();
            camPointHolder = cubeSpawn.transform.GetChild(0);
            
        }



        Time.timeScale = 1;
        //Debug Initialize camera
        ChangeCameraState(activeCube.cameraPoints[ActiveCameraPoint], activeCube.transform);

        //Rotate camera to show all sides
        MoveCamera(activeCube.cameraPoints[ActiveCameraPoint]);

        //Initialize combo buffer
        comboBuffer = new List<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetMouseButtonDown(2))
        {


            LevelManager.Instance.CubeEnd = true;



            StartCoroutine(GameManager.Instance.TowerExitSequence());
        }
        //Check for clicks after cube opens
        if(activeCube.CubeOpened && !IsPointerOverUIObject("UI") && Input.GetMouseButtonDown(0))
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

    //Swipe manager execution
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
        //else if (SwipeManager.Instance.IsSwiping(SwipeDirection.None))
        //{

        //    //Debug.Log("NONE");
        //}
    }




    //Character movement through input
    public void MoveCharacter(int crossDir)
    {
        Vector3 dir = camPointHolder.GetChild(ActiveCameraPoint)
               .transform.position - new Vector3(activeCube.transform.position.x, camPointHolder
                   .GetChild(ActiveCameraPoint).transform.position.y, activeCube.transform.position.z);

        
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
        character.SmoothRotate(tmpDir);
        character.transform.GetComponent<Rigidbody>().velocity = tmpDir;

    }


    //Reset camera transform and set Parent
    public void ChangeCameraState(Transform cameraParent, Transform target)
    {
        
        camPointHolder = cameraParent.parent;
        ActiveCameraPoint = cameraParent.GetSiblingIndex();
        camHolder.SetParent(cameraParent);
        camHolder.GetComponent<CinemachineVirtualCamera>().m_LookAt = target;
        camHolder.localRotation = Quaternion.identity;
        camHolder.localPosition = Vector3.zero;
        camHolder.localScale = Vector3.one;

        StartCoroutine(StartCameraPan());
    }

  
    //Camera switching 
    public void MoveCamera(Transform cameraPoints, int target = -1)
    {
      
        StartCoroutine(StopCamera(cameraPoints, target));
    }

    //Move camera to next position or to target
    public IEnumerator StopCamera(Transform cameraPoints, int target)
    {
       
        //Automated camera move
        if (target == -1)
        {
            ActiveCameraPoint++;

            camHolder.SetParent(cameraPoints.GetChild(ActiveCameraPoint));
            StartCoroutine(CameraLerp(camHolder));
        }
        else
        if (target != -1 && Mathf.Abs(ActiveCameraPoint - target) == 2 /*&& selectedColor == -1*/)
        {
            while (ActiveCameraPoint != target)
            {
                ActiveCameraPoint++;
                Debug.Log("<<<<<<<<<<<<<<<<" + ActiveCameraPoint + " :: " + target);

              
                camHolder.SetParent(cameraPoints.GetChild(ActiveCameraPoint));
                yield return new WaitForSecondsRealtime(0.7f);
                StartCoroutine(CameraLerp(camHolder));
            }
        }
        ////Move to a target cam point
        //else if(target != -1 && Mathf.Abs(ActiveCameraPoint-target) == 1)
        //{

        //    Debug.Log(">>>>>>>>>>>>>>>>>>>>>" + ActiveCameraPoint + " :: " + target);
        //    if (ActiveCameraPoint == cameraPoints.childCount-1 && target == 0)
        //    {
        //        ActiveCameraPoint++;
               
        //    }
        //    else if(ActiveCameraPoint == 0 && target == cameraPoints.childCount-1)
        //    {
        //        ActiveCameraPoint--;
               
        //    }
        //    else if( ActiveCameraPoint > target)
        //    {
        //        ActiveCameraPoint--;
        //    }
        //    else
        //    {
        //        ActiveCameraPoint++;
        //    }

        //    Debug.Log("ACTIVE " + ActiveCameraPoint);
        //    camHolder.SetParent(cameraPoints.GetChild(ActiveCameraPoint));
        //    StartCoroutine(CameraLerp(camHolder));
        //}

    }


    //Camera lerp
    public IEnumerator CameraLerp(Transform camera)
    {
        //Smoothly move camera to point
        while (camera.transform.localPosition != new Vector3(0f, 0f, 0f))
        {
            camera.localPosition = Vector3.Lerp(camera.transform.localPosition, Vector3.zero, cameraSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator StartCameraPan()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 2; i++)
        {
            MoveCamera(camPointHolder);
            yield return new WaitForSeconds(0.8f);
        }
    }

    // Is touching ui check
    public bool IsPointerOverUIObject(string obj)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
            return results[0].gameObject.CompareTag(obj);
        else
            return false;
    }



    public int HighlighLink(Transform tile, bool selectedBool = false)
    {
        //Reload for just check for already selectedColor from links
        if (selectedBool)
        {
            CubeElemController tmpTile = tile.GetComponent<CubeElemController>();
           

            return -1;
        }
        //Actual remembering first color
        else
        {
            //Get indexes of left camera point and right one
            int tmpActiveCamPrev = ActiveCameraPoint - 1;
            if (tmpActiveCamPrev < 0)
                tmpActiveCamPrev = camPointHolder.childCount - 1;
            int tmpActiveCamNext = ActiveCameraPoint + 1;
            if (tmpActiveCamNext >= camPointHolder.childCount)
                tmpActiveCamNext = 0;



            Debug.Log(tmpActiveCamPrev + " : " + tmpActiveCamNext + "::::" + ActiveCameraPoint);

            CubeElemController tmpTile = tile.GetComponent<CubeElemController>();

            for (int i = 0; i < tmpTile.BottomLinks.Count; i++)
            {
                Debug.Log("CHECK " + tmpTile.BottomLinks[i].SideName + "::::" + camPointHolder.GetChild(ActiveCameraPoint).name);

                //Check if color is in front of the camera
                if (tmpTile.BottomLinks[i].SideName == camPointHolder.GetChild(ActiveCameraPoint).name)
                {
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[i].transform.GetComponent<Renderer>().material;

                    int tmpColorIndex = tmpTile.BottomLinks[i].ElemMatIndex;
                    return tmpColorIndex;
                }
                //Or color link is to the left to the camera
                else if (tmpTile.BottomLinks[i].SideName == camPointHolder.GetChild(tmpActiveCamPrev).name)
                {
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[i].transform.GetComponent<Renderer>().material;

                    int tmpColorIndex = tmpTile.BottomLinks[i].ElemMatIndex;
                    return tmpColorIndex;
                }
                //Or color link is to the right to the camera
                else if (tmpTile.BottomLinks[i].SideName == camPointHolder.GetChild(tmpActiveCamNext).name)
                {
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[i].transform.GetComponent<Renderer>().material;

                    int tmpColorIndex = tmpTile.BottomLinks[i].ElemMatIndex;
                    return tmpColorIndex;
                }
                else
                {
                    Debug.Log("else " + tmpTile.BottomLinks[0].SideName);
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[0].transform.GetComponent<Renderer>().material;
                    
                }
            }


            //Remember selection
            foreach (Transform camPoint in activeCube.cameraPoints)
            {

                if (camPoint.name == tmpTile.BottomLinks[0].SideName)
                {
                    Debug.Log("MOVECAM " + activeCameraPoint + " : " + camPoint.GetSiblingIndex());

                    MoveCamera(camPointHolder, camPoint.GetSiblingIndex());
                    break;
                }
            }

            return tmpTile.BottomLinks[0].ElemMatIndex;

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


                //Remember the color
                selectedColor = HighlighLink(tile);
                //Check if there's selected color in bottomLinks to add scorecount
                int bottomColor = FindBottomLink(tmpTile, selectedColor);

                //Remember color count
                comboBuffer.Add(tile);
                //Debug.Log("REE");
                comboCount++;

                //Light up links
                foreach (CubeElemController item in tmpTile.BottomLinks)
                {
                    if (item.transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                    {
                        item.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material.color * 2f);
                    }

                }

                //Debug.Log(comboCount + " : " + activeCube.colorCombos[selectedColor].Count);
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
                //Debug.Log(comboCount + " : " + activeCube.colorCombos[selectedColor].Count);

                //Check for camera turn if needed
                int tmpColor = HighlighLink(tile,true);
                //Check if there's selected color in bottomLinks - grab material from it
                int bottomColor = FindBottomLink(tmpTile, selectedColor);
                //Debug.Log(":>: " + bottomColor + " :: " + selectedColor);
               

                if (bottomColor >=0 && tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                {


                    
                    //////Paint elem to selectedColor if it's same color from links
                    tile.GetComponent<CubeElemController>().materialInteraction = tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material;

                    //Light up links
                    foreach (CubeElemController item in tmpTile.BottomLinks)
                    {
                        if(item.transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                        {
                            item.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", tmpTile.BottomLinks[bottomColor].transform.GetComponent<Renderer>().material.color * 2f);
                        }
                        
                    }
                    



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
    public void ClearBuffer(Transform currentTile = null)
    {
       //Each elem in buffer
        foreach (Transform elem in comboBuffer)
        {


          
            //If combo more than colors - clear color from sides
            if(selectedColor != -1 && comboCount >= activeCube.colorCombos[selectedColor].Count)
            {

                #region TUTORIAL
                if (TutorialManager.Instance && TutorialManager.Instance.TutorialActive == 1)
                {
                    Debug.Log("RzEEEEEEE");
                    TutorialManager.Instance.TutorialActive = 2;
                    TutorialManager.Instance.CloseTut(2);
                }
                else if (TutorialManager.Instance && TutorialManager.Instance.TutorialActive == 3)
                {
                    Debug.Log("RzEEEEEEE");
                    TutorialManager.Instance.TutorialActive = 4;
                    TutorialManager.Instance.CloseTut(4);
                }
                #endregion


                
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
                        
                        ////////////////Delight links
                        //////////////link.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", link.transform.GetComponent<CubeElemController>().ElemMat.color * 0f);

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


                //Remove light up glow from links
                foreach (CubeElemController item in elem.GetComponent<CubeElemController>().BottomLinks)
                {
                    if (item.transform.GetComponent<Renderer>().material.color == activeCube.materials[selectedColor].color)
                    {
                        item.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", item.transform.GetComponent<CubeElemController>().ElemMat.color * 1f);
                    }

                }
            }
        }

          
        //Delete selected colors from links
        if (selectedColor != -1 && comboCount >= activeCube.colorCombos[selectedColor].Count)
        {
            activeCube.colorCombos[selectedColor].Clear();

            //Reset combo counts
            selectedColor = -1;
            comboCount = 0;
            comboBuffer.Clear();
            //Check if cube is clear
            CheckCubeEnd();
            //Delay bottom check if combo is full
            if(!activeCube.EndCube && currentTile != null)
                StartCoroutine(StopBottomCheck(currentTile));
        }
        else
        {
            //Reset ComboCounts
            selectedColor = -1;
            comboCount = 0;
            comboBuffer.Clear();
            //Check bottom right away
            if (currentTile != null)
                BottomCheck(currentTile);
        }

        
        

        

        #region TUTORIAL
        //Skip multiple color if in tutorial
        if(activeCube.TutorialCube)
        {
            return;
        }
        #endregion
        //Check color of current tile if combo is broken
        
       
       
    }
        

    public IEnumerator StopBottomCheck(Transform currentTile)
    {
        yield return new WaitForSeconds(0.4f);
        BottomCheck(currentTile);
    }
    
    //Check if cube is finished
    public void CheckCubeEnd()
    {
        //Debug
        Debug.Log("_____________________");

        int nullCount = 0;
        for (int i = 0; i < activeCube.colorCombos.Length; i++)
        {

            if (activeCube.colorCombos[i] != null)
            {
                if (activeCube.colorCombos[i].Count == 0)
                    nullCount++;
                Debug.Log(activeCube.colorCombos[i].Count + " : " + activeCube.materials[i]);
            }
            else
            {
                nullCount++;
                Debug.Log("NULL");
            }
        }
        Debug.Log("_____________________");

        //Cube Open condition
        if (nullCount >= activeCube.colorCombos.Length)
        {

            //Cube Finish

            LevelManager.Instance.CurrentCube++;
            StartCoroutine(TowerExitSequence());

         
        }

        #region TUTORIAL
        if (TutorialManager.Instance && TutorialManager.Instance.TutorialActive == 4)
        {
            Debug.Log("RzEEEEEEE");
            TutorialManager.Instance.TutorialActive = 4;
            TutorialManager.Instance.CloseTut(4);
        }
        #endregion

    }



    public IEnumerator TowerExitSequence()
    {
        activeCube.anim.SetTrigger("Open");

        yield return new WaitForSeconds(0.7f);
        //Move camera to elevator
        elVcam.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        character.TowerJumpIn(elevator.transform, 2f);
     
        yield return new WaitForSeconds(1f);
        character.SmoothRotate(Vector3.right);
        activeCube.anim.Play("CubeClose");
        yield return StartCoroutine(StopLoadTransition("Tower", 1.4f));
    }

    public IEnumerator StopLoadTransition(string scene, float time)
    {

        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(scene);
    }
    //Find color from bottomLinks
    public int FindBottomLink(CubeElemController tile, int color)
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
