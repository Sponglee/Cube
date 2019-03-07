using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{


    //Reference to animations
    public Animator charAnim;
    //Player input trigger
    public bool InputMove = false;
    //JumpToggle
    public bool JumpBool = false;

    //Move To Tile Speed
    public float tileCenterSpeed = 0.04f;



    //destination
    private Vector3 destination = Vector3.zero;
    public Vector3 Destination
    {
        get => destination;
        set
        {
            destination = value;
            if(value == Vector3.zero)
            {
                //Debug.Log("STOP");

                int pose = Random.Range(0, 4);
                //charAnim.SetInteger("RandomIdle", pose);
                charAnim.SetBool("Moving", false);
                //transform.LookAt(GameManager.Instance.camHolder.position);
            }
            else
            {
                if(!JumpBool)
                {
                    charAnim.SetBool("Moving", true);
                    charAnim.Play("runStart");

                }
                else
                {
                    charAnim.SetBool("Moving", true);
                    charAnim.Play("Jump");
                }
              
            }
        }
    }


   

   

    // Update is called once per frame
    void Update()
    {
        if(Destination != Vector3.zero)
        {
            transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));
            Move();
        }
    }


    public void Move()
    {
        //Debug.Log("RE");
        transform.position = Vector3.MoveTowards(transform.position, Destination, tileCenterSpeed);
        if(Mathf.Approximately(transform.position.x,Destination.x) && Mathf.Approximately(transform.position.z, Destination.z))
        {
            Destination = Vector3.zero;
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Door"))
        {
            if (SceneManager.GetActiveScene().name == "Tower")
            {

                gameObject.SetActive(false);
                //FadeCanvas.Instance.FadeOut(1.3f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Tile") && other.transform.GetComponent<CubeElemController>().SideName == "CubeBottom" && InputMove)
        {
            InputMove = false;
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destination = Vector3.zero;
            Destination = other.transform.position + new Vector3(0, 0f, 0);
            GameManager.Instance.BottomCheck(other.transform);
        }
        else if (other.transform.CompareTag("Door"))
        {
            //if(SceneManager.GetActiveScene().name == "Tower")
            //{

            //    gameObject.SetActive(false);
            //    //FadeCanvas.Instance.FadeOut(1.3f);
            //}




            #region TUTORIAL
            //Check if door is for tutorial - change scene
            if (other.transform.parent.parent.parent.GetComponent<CubeController>().TutorialCube)
            {
                SceneManager.LoadScene("Main");
            }
            #endregion
            //Stop movement
            Destination = Vector3.zero;
            
            //Set new active cube and comboBuffer for GameManager
            CubeController activeCube;
            //Activate a cube
            activeCube = other.transform.parent.parent.parent.GetComponent<CubeController>();
            GameManager.Instance.activeCube = activeCube;
            //Debug Initialize camera
            GameManager.Instance.ChangeCameraState(activeCube.cameraPoints[GameManager.Instance.ActiveCameraPoint], activeCube.transform);
            //Initialize combo buffer
            GameManager.Instance.comboBuffer = new List<Transform>();
            transform.position = activeCube.transform.position + new Vector3(0, 0.3f, 0);

        }


      
    }



    public void TowerJumpIn(Transform target, float speed)
    {
        Vector3 tarDir = (target.position - transform.position)*speed;
        StartCoroutine(StopJumpIn(tarDir));  
    }


    public IEnumerator StopJumpIn(Vector3 tmpDir)
    {
        

        yield return StartCoroutine(StopSmoothRotate(tmpDir));
        tmpDir += Vector3.up * 2.5f;
        transform.GetComponent<Rigidbody>().velocity = tmpDir;
        charAnim.Play("Jump");
    }


    public void SmoothRotate(Vector3 tmpDir)
    {
        StartCoroutine(StopSmoothRotate(tmpDir));
    }
    public IEnumerator StopSmoothRotate(Vector3 tmpDir)
    {
        float AngleToFace = Vector3.Angle(Vector3.forward, tmpDir);
      
        tmpDir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(tmpDir);

        float duration = 0.3f;
        float i = 0f;
        float rate = 1f / duration;
        //move Camera
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;


            // slerp to the desired rotation over time
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, i);
            yield return null;
        }
    }



    //private void OnTriggerEnter (Collider collision)
    //{

    //}

}
