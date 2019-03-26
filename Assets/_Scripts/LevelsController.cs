using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LevelsController : Singleton<LevelsController>
{
    public Transform FollowTarget;

    public Transform currentCanvas;

    public Camera physicCam;
    
    //Scene cameraPair
    public Transform cameraHolder;
    public Transform elevatorHolder;

    public PlayableDirector director;

    //Swipe variables
    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;
    [SerializeField]
    private float swipeResistance = 50f;

    // Start is called before the first frame update
    void Start()
    {
        //Conditions for tower pan sequence
        if(ProgressManager.Instance.TowerExit)
        {
            
        }
        else if(ProgressManager.Instance.TowerManualExit)
        {
            ProgressManager.Instance.TowerManualExit = false;
        }
        else
        {
            director.enabled = false;
           
        }
      
        //SetCamera and elevator to current Tower
        Vector3 tmpPos = new Vector3(cameraHolder.position.x, cameraHolder.position.y, transform.GetChild(0).GetChild(ProgressManager.Instance.CurrentTower).position.z);
        cameraHolder.position = tmpPos;
        elevatorHolder.position = new Vector3(elevatorHolder.position.x, elevatorHolder.position.y, tmpPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //Remember start touch position (SwipeManager replacement)
                startTouch = touch.position;
                screenTouch = physicCam.ScreenToViewportPoint(startTouch);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endTouch = physicCam.ScreenToViewportPoint(touch.position);
                Vector3 deltaSwipe = screenTouch - endTouch;


                //Door click event
                if (currentCanvas && currentCanvas.gameObject.activeSelf && Mathf.Abs(deltaSwipe.y) <= swipeResistance)
                {
                    //Enable phys camera trigger
                    //physicCam.GetComponentInChildren<SphereCollider>().enabled = true;
                    GameObject tmp = GrabRayObj();
                    //Debug.Log(tmp.tag);


                    if (tmp && tmp.CompareTag("Door") && Mathf.Approximately(elevatorHolder.position.z, FollowTarget.position.z) 
                        &&  Mathf.Approximately(cameraHolder.position.z, FollowTarget.position.z))
                    {


                        if (ProgressManager.Instance.CurrentTower <= ProgressManager.Instance.levelProgress)
                        {
                            //Remember what tower u picked
                            ProgressManager.Instance.towerIndex = tmp.transform.parent.GetSiblingIndex();
                            SceneManager.LoadScene("Tower");
                        }
                    }
                }

                Vector3 tmpPos = new Vector3(cameraHolder.position.x, cameraHolder.position.y, FollowTarget.position.z);

                //For camera
                StartCoroutine(StopLook(cameraHolder, tmpPos, 0.2f));
                //For elevator
                StartCoroutine(StopLook(elevatorHolder, new Vector3(elevatorHolder.position.x, elevatorHolder.position.y, tmpPos.z), 1f));
            }
        }

    }

    public void MoveToNextTower(int targetTower)
    {
        //SetCamera and elevator to current Tower
        Vector3 tmpPos = new Vector3(cameraHolder.position.x, cameraHolder.position.y, transform.GetChild(0).GetChild(targetTower).position.z);
        StartCoroutine(StopLook(cameraHolder, tmpPos, 0.2f));
        StartCoroutine(StopLook(elevatorHolder, new Vector3(elevatorHolder.position.x, elevatorHolder.position.y, tmpPos.z), 1f));
    }



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




    //Tower pick
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
                if (hit.collider.CompareTag("Door"))
                {
                    GameObject objectHit = hit.transform.gameObject;
                    Debug.Log(objectHit.tag);
                    return objectHit;

                }

            }
        }


        return null;
    }
}
