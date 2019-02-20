using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerController : MonoBehaviour
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
    public Transform FollowTarget;

    public Transform currentCanvas;

    //Swipe variables
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;

    [SerializeField]
    private float swipeResistance = 50f;



    private void Start()
    {
        
    }




    private void Update()
    {

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

                if (currentCanvas && currentCanvas.gameObject.activeSelf)
                {
                    GameObject tmp = GrabRayObj();

                    if (tmp && tmp.CompareTag("Cube") && elevatorHolder.position == cameraHolder.position)
                    {
                        characterController.TowerJumpIn(tmp.transform);
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
    }


    public IEnumerator StopLoadTransition(string scene)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(scene);
    }



    //Elem pick
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



    public IEnumerator StopLook(Transform target, Vector3 tmpPos, float duration)
    {
        float i = 0f;
        float rate = 1f / duration;
        //move Camera
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            target.position = Vector3.Lerp(target.position, tmpPos, i);

            yield return null;
        }


    }

    

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("MainCamera"))
        {
            Debug.Log("REEE");
        }
        
    }



}
