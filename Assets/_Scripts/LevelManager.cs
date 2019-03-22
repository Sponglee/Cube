﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{

    public TowerData twrData;


    //Data holder
    public CubeData cubeInfo;
    //Material pool (for length)
    public Material[] materials;

    //Cube exit sequence bool
    public bool CubeEnd = false;
    public bool TowerEnd = false;
   
    //Current tower index
    public int towerIndex = 0;


    //Tower Controller reference
    [SerializeField]
    private TowerController towerController;
    ////Selected cube progress
    //[SerializeField]
    //private int towerProgress = 0;


    [SerializeField]
    private int currentCube = 0;
    public int CurrentCube
    {
        get => currentCube;
        set
        {
            //If current level is progressing - save progress
            if(value > twrData.twrProgress)
            {
                currentCube = value;
                EndTowerCheck(value);
            }
            else if(value == twrData.twrProgress && value == twrData.twrCubeCount)
            {
                currentCube = value;
                EndTowerCheck(value);
                
            }
            else
            {
                currentCube = value;
                

            }

        }
    }

   
    //Check forr endTower
    public void EndTowerCheck(int value)
    {
        //Check for endTower
        if (twrData != null && twrData.twrCubeCount != 0)
        {
            twrData.twrProgress = value;
            Debug.Log(value + " : :: : " + twrData.twrProgress + " : :: : " + twrData.twrCubeCount);
            //Enable Next Tower Bool
            if (twrData.twrProgress == twrData.twrCubeCount)
            {
                TowerEnd = true;
                //Enable FINISH tagged exit tower trigger
                TowerController.Instance.TowerEndTrigger = TowerController.Instance.TowerGrid.parent.GetChild(1).GetChild(0).gameObject;
                TowerController.Instance.TowerEndTrigger.SetActive(true);
            }
            Debug.Log("CURRENTCUBE SAVE");
            SaveSystem.SaveLevel(towerIndex, twrData, true);

        }

    }

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("AWAKEEEEEE " + gameObject.name);
        
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Debug.Log("DESTORYED " + gameObject.name);
            Destroy(gameObject);

        }
        //Make sure only 1 isntance is up
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        Debug.Log("STAAARTT " + gameObject.name);

        //Load tower data, if there's none - randomize it
        twrData = SaveSystem.LoadLevel(towerIndex);

        if (twrData == null)
        {
            //Load backup 
            //*** HERE ***
            RandomizeTower();
            CurrentCube = 0;



        }
        else if(twrData.twrCubeCount != 0)
        {

            Debug.Log("CurrentCubeLOAD");
            CurrentCube = twrData.twrProgress;
            if (CurrentCube > twrData.twrCubeCount)
            {
                CurrentCube = twrData.twrCubeCount;
            }
        }
        //File bug proof
        else
        {
            Debug.Log("____FILESAFE__________________________________________________________________________________________________________");
            //LOAD FILE WITH BACKUP
            SaveSystem.LoadLevel(towerIndex, true);
            CurrentCube = 0;
        }



        
        //Initialize tower
        TowerController.Instance.InitializeTower(twrData);

        //Set character pair to current cube in tower
        if (CurrentCube >= twrData.twrCubeCount)
        {
            SetCamera(CurrentCube-1);
        }
        else
        {
            SetCamera(CurrentCube);
        }

        //Check for endTower to enable exit trigger
        EndTowerCheck(CurrentCube);
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if(CurrentCube == twrData.twrCubeCount)
                CurrentCube--;
            StartCoroutine(ProgressionMoveUp());
        }
    }


    //Scene loaded delegates
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

  

    //When Scene is loaded (only after another scene with levelManager preset, not first time)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("RE " + gameObject.name);
       

        if (scene.buildIndex == 2)
        {
            TowerController.Instance.InitializeTower(twrData);
           
            if (CubeEnd)
            {

                Debug.Log(CurrentCube + " !#@$@1$!");
                //**************************************
                //Set character pair to current cube in tower
                SetCamera(CurrentCube);

                StartCoroutine(ProgressionMoveUp());

                CubeEnd = false;

            }
            else
            {
                CurrentCube = twrData.twrProgress;
                Debug.Log("CAMERA   " + gameObject.name + " : "  + CurrentCube + " : : : " + TowerController.Instance.TowerGrid.childCount);
                
                
                //***************************************
                //Set character pair to current cube in tower and Canvas
                SetCamera(CurrentCube);
               

            }


            //Check for endtower to enable exit trigger
            EndTowerCheck(CurrentCube);


        }
    }


    public void SetCamera(int curCubeTarget)
    {
        Debug.Log("CURCUBE " + curCubeTarget + " : " + TowerController.Instance.TowerGrid.childCount);
        if (TowerController.Instance.TowerGrid.childCount != 0)
        {
            //Set camera to grid child
            TowerController.Instance.cameraHolder.position = TowerController.Instance.TowerGrid.GetChild(curCubeTarget).position + Vector3.up * 0.6f;

            //Enable Canvas for currentCube
            TowerController.Instance.TowerGrid.GetChild(curCubeTarget).GetChild(1).gameObject.SetActive(true);
        }
    }

    public IEnumerator ProgressionMoveUp()
    {
        Debug.Log("UPPPPP");
        yield return new WaitForSeconds(2f);

        CurrentCube++;

        yield return new WaitForEndOfFrame();

        if (TowerEnd)
        {
            StartCoroutine(TowerEndSequence());
        }
        else
        {


            TowerController tmpCont = TowerController.Instance;

            //***************************************
            //Set FOLLOW TARGET to current cube in tower
            if (CurrentCube >= twrData.twrCubeCount)
            {
               
            }
            else
            {
                tmpCont.FollowTarget.position = tmpCont.TowerGrid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

                //Enable Canvas for currentCube
                tmpCont.TowerGrid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);
            }


            Vector3 tmpPos = new Vector3(tmpCont.cameraHolder.position.x, tmpCont.FollowTarget.position.y, tmpCont.cameraHolder.position.z);
            //For camera
            StartCoroutine(tmpCont.StopLook(tmpCont.cameraHolder, tmpPos, 0.5f));
            //For elevator
            StartCoroutine(tmpCont.StopLook(tmpCont.elevatorHolder, new Vector3(tmpCont.elevatorHolder.position.x, tmpPos.y, tmpCont.elevatorHolder.position.z), 1.5f));

           
        }

        

      

    }



    public IEnumerator TowerEndSequence ()
    {
        TowerController towerCont = TowerController.Instance;

        //Move elevator upstairs
        towerCont.FollowTarget.position = towerCont.TowerGrid.parent.GetChild(1).position;
        //towerCont.cameraHolder.position = towerCont.TowerGrid.parent.GetChild(1).GetChild(0).position;

        //Enable Canvas for currentCube
        towerCont.TowerGrid.GetChild(CurrentCube - 1).GetChild(1).gameObject.SetActive(false);

        Vector3 tmpPos = new Vector3(towerCont.cameraHolder.position.x, towerCont.FollowTarget.position.y, towerCont.cameraHolder.position.z);


        //For elevator
        StartCoroutine(towerCont.StopLook(towerCont.elevatorHolder, new Vector3(towerCont.elevatorHolder.position.x, tmpPos.y, towerCont.elevatorHolder.position.z), 6f));



        yield return new WaitForSecondsRealtime(2f);

        //Set follow object to null
        towerCont.vcam.m_Follow = null;
        //Move Camera trigger in elevator pair to top, enable endtower camera 
        towerCont.vcamTowerEnd.gameObject.SetActive(true);
      
        //Move Camera trigger up
        //StartCoroutine(towerCont.StopLook(towerCont.cameraHolder, new Vector3(towerCont.cameraHolder.position.x, tmpPos.y, towerCont.cameraHolder.position.z), 5f));
        //FadeCanvas.Instance.FadeOut(0.5f);



       
        //SceneManager.LoadScene("Levels");
        TowerEnd = false;
    }


    //Generate tower cubes
    public void RandomizeTower()
    {
        twrData = new TowerData();
        twrData.cubes = new List<CubeData>();
        twrData.twrProgress = 0;
        //Generate amount of cubes
        twrData.twrCubeCount = Random.Range(3, 4);

        for (int t = 0; t < twrData.twrCubeCount; t++)
        {

            cubeInfo = new CubeData();
            cubeInfo.type = 3;

            //For each side (4 - right , back, left, front)
            for (int i = 0; i < cubeInfo.sides.Length; i++)
            {
                //Debug.Log("once");
                //Set and remember random color per side
                int randomMat = Random.Range(1, materials.Length);

                //Remember color for a side 
                cubeInfo.sides[i].sideMat = randomMat;
                cubeInfo.sides[i].elemColors = new int[cubeInfo.type * cubeInfo.type];

                for (int j = 0; j < cubeInfo.type * cubeInfo.type; j++)
                {
                    //Randomize side's elem colorss
                    float matRandomizer = Random.Range(0, 100);

                    //If checked - get a randomMat to elem, if not - leave 0
                    if (matRandomizer <= 50)
                    {
                        cubeInfo.sides[i].elemColors[j] = randomMat;

                    }
                    else
                    {
                        cubeInfo.sides[i].elemColors[j] = 0;
                    }

                    //Debug.Log(cubeInfo.sides[i].elemColors.Length);
                }

            }
            //Debug.Log("====");
            twrData.cubes.Add(cubeInfo);
        }


        Debug.Log("RANDOMIZE SAVE");
        SaveSystem.SaveLevel(towerIndex, twrData, true);

        ////Randomize obstacles
        //float obsRandomizer = UnityEngine.Random.Range(0, 100);

        //if (obsRandomizer >= 80)
        //{
        //    int sideRandomizer = UnityEngine.Random.Range(0, cubeSides.Length);

        //    //Transform randSide = cubeSides[sideRandomizer].GetChild(0).GetChild(0);
        //    Transform randSide = cubeBottom.GetChild(0).GetChild(0);

        //    GameObject tmpObs = Instantiate(obstaclePref);

        //    ObstacleController obs = tmpObs.GetComponent<ObstacleController>();

        //    obs.cube = this;
        //    obs.startPoint = randSide.GetChild(UnityEngine.Random.Range(0, randSide.childCount));
        //    obs.endPoint = randSide.GetChild(UnityEngine.Random.Range(0, randSide.childCount));


        //}


    }

}
