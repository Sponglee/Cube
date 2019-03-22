using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerController : Singleton<TowerController>
{
    public float rotSpeed = 20;
    public float scrollSpeed = 2;

    public float rotResistance = 5;
    public Transform cameraHolder;
    public Transform elevatorHolder;

    public CharacterController characterController;

    public TowerData data;



    public Camera physicCam;
    public CinemachineVirtualCamera vcam;
    //Camera for enter
    public CinemachineVirtualCamera vdoor;
    //Camera for end 
    public CinemachineVirtualCamera vcamTowerEnd;
    //For exiting the tower
    public GameObject TowerEndTrigger;
    public Transform FollowTarget;

    

    public Transform currentCanvas;

    //Swipe variables
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;

    [SerializeField]
    private float swipeResistance = 50f;

    public Transform activeTower;
    public Transform TowerGrid;


    private void Start()
    {
        //Zoom out sequence
        //StartCoroutine(StartDelay());
       
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForEndOfFrame();
        /////// USE THIS IF EXITING MAIN SCENE
        //Disable door camera for pan out shot
        vdoor.Priority -= 2;
    }



    private void Update()
    {
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonDown(2))
//        {

//            GenerateTower(LevelManager.Instance.twrData);

//            //Set character pair to current cube in tower
//            TowerController.Instance.cameraHolder.position = TowerController.Instance.TowerGrid.GetChild(LevelManager.Instance.CurrentCube).position + Vector3.up * 0.6f;

//            //Enable Canvas for currentCube
//            TowerController.Instance.TowerGrid.GetChild(LevelManager.Instance.CurrentCube).GetChild(1).gameObject.SetActive(true);

//            ////Door click event
//            //if (currentCanvas && currentCanvas.gameObject.activeSelf)
//            //{
//            //    //Enable phys camera trigger
//            //    physicCam.GetComponentInChildren<SphereCollider>().enabled = true;
//            //    GameObject tmp = GrabRayObj();

//            //    if (tmp && tmp.CompareTag("Cube") && elevatorHolder.position == cameraHolder.position)
//            //    {

//            //        //Hide Tower
//            //        transform.GetChild(1).gameObject.SetActive(false);
//            //        //Disable all other cubes
//            //        foreach (Transform child in transform.GetChild(0))
//            //        {
//            //            if (child.gameObject != tmp)
//            //            {
//            //                child.gameObject.SetActive(false);
//            //            }

//            //        }


//            //        StartCoroutine(TowerEnterSequence(tmp));




//            //    }
//            //}
//        }
//#endif
        if (Input.touchCount > 0)
        {
            //Touch touch = Input.GetTouch(0);
            //if (touch.phase == TouchPhase.Began)
            //{
            //    //Remember start touch position (SwipeManager replacement)
            //    startTouch = touch.position;
            //    screenTouch = physicCam.ScreenToViewportPoint(startTouch);
            //}
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                //endTouch = physicCam.ScreenToViewportPoint(touch.position);
                //Vector3 deltaSwipe = screenTouch - endTouch;

                //if (Mathf.Abs(deltaSwipe.y) <= swipeResistance)
                //{

                //}


                //Door click event
                if (currentCanvas && currentCanvas.gameObject.activeSelf)
                {
                    //Enable phys camera trigger
                    physicCam.GetComponentInChildren<SphereCollider>().enabled = true;
                    GameObject tmp = GrabRayObj();

                    if (tmp && tmp.CompareTag("Cube") && elevatorHolder.position.y == cameraHolder.position.y)
                    {
                       
                        if (tmp.transform.GetSiblingIndex() <= ProgressManager.Instance.twrData.twrProgress)
                        {
                            ProgressManager.Instance.CurrentCube = tmp.transform.GetSiblingIndex();
                            //Hide Tower
                            activeTower.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
                            //Disable all other cubes
                            foreach (Transform child in activeTower.GetChild(0).GetChild(0).GetChild(0))
                            {
                                if (child.gameObject != tmp)
                                {
                                    child.gameObject.SetActive(false);
                                }

                            }
                            StartCoroutine(TowerEnterSequence(tmp));
                        }
                        else
                        {
                            Debug.Log("NOT UNLOCKED " + tmp.transform.GetSiblingIndex());
                        }

                        



                       
                    }
                }

                Vector3 tmpPos = new Vector3(cameraHolder.position.x, FollowTarget.position.y, cameraHolder.position.z);

                //For camera
                StartCoroutine(StopLook(cameraHolder, tmpPos, 0.2f));
                //For elevator
                StartCoroutine(StopLook(elevatorHolder, new Vector3(elevatorHolder.position.x, tmpPos.y, elevatorHolder.position.z), 1f));
            }
        }


    }


    public IEnumerator TowerEnterSequence(GameObject towerCube)
    {
        towerCube.GetComponent<Animator>().SetBool("TowerOpen", true);
        yield return new WaitForSeconds(0.3f);

        characterController.TowerJumpIn(towerCube.transform, 2.3f);
        //Zoom in
        vdoor.Priority += 2;


    }



    public GameObject nodePref;
    public float nodeStep = 1.4f;

    public void InitializeTower(TowerData twrData)
    {
        int gridStep = 0;

        foreach (CubeData cube in twrData.cubes)
        {
            GameObject tmpNode = Instantiate(nodePref, TowerGrid.position + Vector3.up * nodeStep*gridStep, Quaternion.identity, TowerGrid);
            gridStep++;
        }
    }


    //Scroll towerf planet
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

        if (rotX > rotResistance)
        {
            cameraHolder.transform.Rotate(Vector3.up, rotX, Space.Self);
        }

        //Scroll camera and elevator
        cameraHolder.transform.position += new Vector3(0, -rotY / 100f, 0);
        elevatorHolder.transform.position += new Vector3(0, -rotY / 120f, 0);
        Debug.Log(rotY);
    }


    public IEnumerator StopLoadTransition(string scene)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(scene);
    }



    //Cube pick
    public GameObject GrabRayObj()
    {



        Vector3 rayPos = Input.mousePosition;
        rayPos.z = physicCam.farClipPlane;

        rayPos = physicCam.ScreenToWorldPoint(rayPos);





        Debug.DrawLine(rayPos, physicCam.transform.position, Color.red, 5f);

        RaycastHit[] hits = Physics.RaycastAll(physicCam.transform.position, rayPos);

        if (hits.Length != 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Cube"))
                {
                    GameObject objectHit = hit.transform.gameObject;
                    Debug.Log(objectHit.tag);
                    return objectHit;

                }

            }
        }


        return null;
    }
   
    //private void OnTriggerExit(Collider other)
    //{

    //    if (other.gameObject.CompareTag("Cube"))
    //    {
    //        other.transform.GetChild(1).gameObject.SetActive(false);
    //    }
    //}



    //Lerp to target location from destination
    public IEnumerator StopLook(Transform target, Vector3 destination, float duration)
    {
        float i = 0f;
        float rate = 1f / duration;
        //move Camera
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            target.position = Vector3.Lerp(target.position, destination, i);

            yield return null;
        }


    }

    

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            //Debug.Log("REEE");
        }
        
    }



}
