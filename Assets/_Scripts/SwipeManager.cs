using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/** Swipe direction */
public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
    UpRight = 16,
    DownRight = 32,
    DownLeft = 64,
    UpLeft = 128,

}



public class SwipeManager : Singleton<SwipeManager>
{


    public SwipeDirection Direction { set; get; }


    private Vector3 startTouch;
    private Vector3 endTouch;
    private Vector3 screenTouch;


    [SerializeField]
    private float swipeResistance = 5f;


    public bool SwipeC = true;
    public bool swipeValue = false;

    //gameManager reference
    [SerializeField]
    private GameManager gameManager;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        Direction = SwipeDirection.None;

        

        if (Input.touchCount > 0 && !gameManager.activeCube.CubeOpened)
        {
            Vector2 deltaSwipe = Vector2.zero;

            Touch[] touches = Input.touches;

            //If there's 2 fingers
            if(touches.Length == 2)
            {
                //Activate jumpbool
                gameManager.character.JumpBool = true;

                //If second finger swipes
                if (touches[1].phase == TouchPhase.Began)
                {
                    //Debug.Log("BEGAN");
                    startTouch = touches[1].position;
                    screenTouch = gameManager.physicalCam.ScreenToViewportPoint(startTouch);
                }
                else if (touches[1].phase == TouchPhase.Ended)
                {
                    endTouch = gameManager.physicalCam.ScreenToViewportPoint(touches[1].position);
                    deltaSwipe = screenTouch - endTouch;
                    //Debug.Log("ENDED " + deltaSwipe.x + ":" + deltaSwipe.y
                    CheckSwipe(deltaSwipe);
                   
                }
            }
            //Else if only 1 finger
            else if(touches.Length == 1)
            {
                Touch touch = Input.GetTouch(0);
                //If jump finger was pressed - release and return
                if (gameManager.character.JumpBool)
                {
                    if (touches[0].phase == TouchPhase.Ended)
                    {
                        //Release jumpbool
                        gameManager.character.JumpBool = false;

                        //Debug.Log("SECOND END");
                        return;
                    }
                }
                //Proceed with 1 finger
                else if (touch.phase == TouchPhase.Began)
                {
                    //Debug.Log("BEGAN");
                    startTouch = touch.position;
                    screenTouch = gameManager.physicalCam.ScreenToViewportPoint(startTouch);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouch = gameManager.physicalCam.ScreenToViewportPoint(touch.position);
                    deltaSwipe = screenTouch - endTouch;
                    //Debug.Log("ENDED " + deltaSwipe.x + ":" + deltaSwipe.y);
                    CheckSwipe(deltaSwipe);
                   
                }
            }
        }
    }


    //Execute the swipe
    public void CheckSwipe(Vector2 deltaSwipe)
    {
        if (Mathf.Abs(deltaSwipe.x) > Mathf.Abs(deltaSwipe.y) && Mathf.Abs(deltaSwipe.x) > swipeResistance)
        {
            if (Mathf.Abs(deltaSwipe.y) > swipeResistance)
            {
                if (deltaSwipe.x < 0)
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpRight : SwipeDirection.DownRight;
                else if (deltaSwipe.x > 0)
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpLeft : SwipeDirection.DownLeft;
            }
            else
                Direction |= (deltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else if (Mathf.Abs(deltaSwipe.y) > Mathf.Abs(deltaSwipe.x) && Mathf.Abs(deltaSwipe.y) > swipeResistance)
        {
            if (Mathf.Abs(deltaSwipe.x) > swipeResistance)
            {
                if (deltaSwipe.x < 0)
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpRight : SwipeDirection.DownRight;
                else if (deltaSwipe.x > 0)
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpLeft : SwipeDirection.DownLeft;
            }
            else
                Direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
        }
        else
        {

            Direction |= SwipeDirection.None;
        }
      
        //Debug.Log(Direction);
        gameManager.CharacterSwipeResult();
    }

    //Remember swipe state
    public bool IsSwiping(SwipeDirection dir)
    {
        return (Direction & dir) == dir;
    }


    //Change swipe value
    public void SwipeChange()
    {
        swipeValue = !swipeValue;

    }
}