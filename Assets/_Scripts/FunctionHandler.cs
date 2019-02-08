using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FunctionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ChangeCamera()
    {
        //Switch cam
        GameManager.Instance.MoveCamera(GameManager.Instance.currentCamPoints);
    }


    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }


    //public void RightClick()
    //{
      
    //    //Jump trigger
    //    GameManager.Instance.character.JumpBool = true;
    //}
}
