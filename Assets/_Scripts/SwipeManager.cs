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


    private Vector3 touchPosition;
    private Vector3 screenTouch;


    [SerializeField]
    private float swipeResistance = 5f;


    public bool SwipeC = true;
    public bool swipeValue = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update ()
    {


        Direction = SwipeDirection.None;
        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
            screenTouch = GameManager.Instance.physicalCam.ScreenToViewportPoint(touchPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 deltaSwipe = touchPosition - Input.mousePosition;



            //if (Mathf.Abs(deltaSwipe.y) > swipeResistance && Mathf.Abs(deltaSwipe.x) > swipeResistance 
            //    && Mathf.Approximately(Mathf.Abs(deltaSwipe.y), Mathf.Abs(deltaSwipe.x)))
            //{

            //    if (deltaSwipe.x < 0)
            //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpRight : SwipeDirection.DownRight;
            //    else if (deltaSwipe.x > 0)
            //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpLeft : SwipeDirection.DownLeft;
            //}
            /*else */if (Mathf.Abs(deltaSwipe.x) > Mathf.Abs(deltaSwipe.y) && Mathf.Abs(deltaSwipe.x) > swipeResistance)
            {
                //if (deltaSwipe.y>swipeResistance)
                //{
                //    if (deltaSwipe.x < 0)
                //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpRight : SwipeDirection.DownRight;
                //    else if (deltaSwipe.x > 0)
                //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpLeft : SwipeDirection.DownLeft;
                //}
                //else
                    Direction |= (deltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else if (Mathf.Abs(deltaSwipe.y) > Mathf.Abs(deltaSwipe.x) && Mathf.Abs(deltaSwipe.y) > swipeResistance)
            {
                //if (deltaSwipe.x > swipeResistance)
                //{
                //    if (deltaSwipe.x < 0)
                //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpRight : SwipeDirection.DownRight;
                //    else if (deltaSwipe.x > 0)
                //        Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UpLeft : SwipeDirection.DownLeft;
                //}
                //else
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
            }
            else
            {

                Direction |= SwipeDirection.None;
            }
            /*D*/
            Debug.Log(Direction);
        }

    }
    public bool IsSwiping(SwipeDirection dir)
    {

        return (Direction & dir) == dir;
    }


    public void SwipeChange()
    {
        swipeValue = !swipeValue;

    }
}