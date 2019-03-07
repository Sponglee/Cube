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
        GameManager.Instance.MoveCamera(GameManager.Instance.camPointHolder);
        #region TUTORIAL
        if (TutorialManager.Instance && TutorialManager.Instance.TutorialActive == 2)
        {
            Debug.Log("RzEEEEEEE");
            TutorialManager.Instance.TutorialActive = 3;
            TutorialManager.Instance.CloseTut(3);
        }
        #endregion
    }


  

    //Menu
    public GameObject menu;

    public void OpenMenu()
    {
        menu.SetActive(true);
        Time.timeScale =1;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    //Tutorial
    public void LoadScene(string sceneName)
    {
       
        SceneManager.LoadScene(sceneName);
    }

    //Exit
    public void Exit()
    {
        Application.Quit();
    }
    
}
