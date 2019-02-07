using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                charAnim.SetInteger("RandomIdle", pose);
                charAnim.SetBool("Moving", false);
                //transform.LookAt(GameManager.Instance.camHolder.position);
            }
            else
            {
                charAnim.SetBool("Moving", true);
                charAnim.Play("runStart");
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
            //Stop movement
            Destination = Vector3.zero;
            
            //Set new active cube and comboBuffer for GameManager
            CubeController activeCube;
            //Activate a cube
            activeCube = other.transform.parent.parent.parent.GetComponent<CubeController>();
            GameManager.Instance.activeCube = activeCube;
            //Debug Initialize camera
            GameManager.Instance.ChangeCameraState(activeCube.cameraPoints[GameManager.Instance.activeCameraPoint], activeCube.transform);
            //Initialize combo buffer
            GameManager.Instance.comboBuffer = new List<Transform>();
            transform.position = activeCube.transform.position + new Vector3(0, 0.3f, 0);

        }


      
    }

    //private void OnTriggerEnter (Collider collision)
    //{
        
    //}

}
