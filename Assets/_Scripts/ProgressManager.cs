using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : Singleton<ProgressManager>
{

    public TowerData twrData;


    //Data holder
    public CubeData cubeInfo;
    //Material pool (for length)
    public Material[] materials;

    //Cube exit sequence bool
    public bool CubeEnd = false;
    public bool TowerExit = false;
    //For exiting to levels manually
    public bool TowerManualExit = false;
    //Tower Controller reference
    [SerializeField]
    private TowerController towerController;


    //Current tower index
    public int towerIndex = 0;
    //Progress of levels
    public int levelProgress = 0;

    [SerializeField]
    private int currentTower = -1;
    public int CurrentTower
    {
        get => currentTower;
        set
        {
            currentTower = value;
            //If Tower completed and its greater than previous levelprogress - set new one
            if (TowerExit && value > PlayerPrefs.GetInt("LevelProgress", 0))
                PlayerPrefs.SetInt("LevelProgress", value);
        }
    }
   


    [SerializeField]
    private int currentCube = 0;
    public int CurrentCube
    {
        get => currentCube;
        set
        {
            currentCube = value;
            if (value >= twrData.twrProgress)
                EndTowerCheck(value);
            ////If current level is progressing - save progress
            //if(value > twrData.twrProgress)
            //{
            //    currentCube = value;
            //    EndTowerCheck(value);
            //}
            ////Return from cube or levels and tower is complete
            //else if(value == twrData.twrProgress && value == twrData.twrCubeCount)
            //{
            //    currentCube = value;
            //    EndTowerCheck(value);

            //}
            //else
            //{
            //    currentCube = value;


            //}

        }
    }

  
    public void CheckLevelLoad()
    {
        if(CurrentTower != -1)
        {
            SceneManager.LoadScene("Levels");
        }
        else if(twrData != null && twrData.twrProgress > 0)
        {
            SceneManager.LoadScene("Tower");
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

    //Check forr endTower
    public void EndTowerCheck(int value)
    {
        //Check for endTower
        if (twrData != null && twrData.twrCubeCount != 0 && value > twrData.twrProgress)
        {
            twrData.twrProgress = value;
            Debug.Log(value + " : :: : " + twrData.twrProgress + " : :: : " + twrData.twrCubeCount);
            //Enable Next Tower Bool
            if (twrData.twrProgress == twrData.twrCubeCount)
            {
                TowerExit = true;

                Debug.Log(levelProgress + " == " + twrData.index);
                if(twrData.index>=levelProgress)
                {
                    Debug.Log(">>>>>" + CurrentTower);
                    //If Tower is finished - increment levelProgress
                    PlayerPrefs.SetInt("LevelProgress", CurrentTower + 1);
                    levelProgress = PlayerPrefs.GetInt("LevelProgress", -1);
                }
                ////Enable FINISH tagged exit tower trigger
                //TowerController.Instance.TowerEndTrigger = TowerController.Instance.Grid.parent.GetChild(1).GetChild(0).gameObject;
                //TowerController.Instance.TowerEndTrigger.SetActive(true);
            }
            Debug.Log("CURRENTCUBE SAVE");
            SaveSystem.SaveLevel(towerIndex, twrData, true);

        }
        else if(twrData != null && twrData.twrCubeCount != 0)
        {

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

        //twrData = null;
    }

    private void Start()
    {
        Debug.Log("STAAARTT " + gameObject.name);

        //Check if tower is new, initialize or generate it
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            if (twrData.twrCubeCount == 0)
            {
                Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<1");

                LoadNewTower();
            }
        }

        //Set Up current Tower to progress if fresh launch
        levelProgress = PlayerPrefs.GetInt("LevelProgress", -1);
        //Set current Tower to progress, if first launch
        if (CurrentTower == -1)
            CurrentTower = levelProgress;
       
    }

    //Initializing new tower or start of the level
    public void LoadNewTower()
    {
        Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<" + gameObject.name);
        //Load tower data, if there's none - randomize it
        twrData = SaveSystem.LoadLevel(towerIndex);

        if (twrData == null)
        {
            //Load backup 
            //*** HERE ***
            RandomizeTower();
            CurrentCube = 0;



        }
        else if (twrData.twrCubeCount != 0)
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



        Debug.Log("NEW LOADDED");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //Initialize tower
            TowerController.Instance.InitializeTower(twrData);


            ////Set character pair to current cube in tower
            if (CurrentCube >= twrData.twrCubeCount)
            {
                SetCamera(CurrentCube - 1);
            }
            else
            {
                SetCamera(CurrentCube);
            }

            //Check for endTower to enable exit trigger
            EndTowerCheck(CurrentCube);
        }


       
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(CurrentCube == twrData.twrCubeCount)
            {
                CubeEnd= true;
                
                CurrentCube--;
            }

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
        Debug.Log("RE " + gameObject.name + " :::::: " + twrData.index);
        levelProgress = PlayerPrefs.GetInt("LevelProgress", -1);

        //Set current Tower to progress, if first launch
        if (CurrentTower == -1)
            CurrentTower = levelProgress;


        if (scene.buildIndex == 1 || scene.buildIndex == 3)
        {
            //First launch of tower Scene (start sequence)
            if (twrData.twrCubeCount == 0)
            {
                Debug.Log("FIRST LAUNCH");
                Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<2");
                LoadNewTower();
                levelProgress = PlayerPrefs.GetInt("LevelProgress", -1);
                
                return;
            }
            else
            {

                if (twrData.index == towerIndex)
                {
                    Debug.Log("LOADDED TOWER");
                    if (scene.buildIndex == 1)
                    {
                        TowerController.Instance.InitializeTower(twrData);
                    }

                }
                else
                {
                    Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<3");
                    LoadNewTower();
                }


            }

            //If exited main scene
            if (CubeEnd)
            {

                Debug.Log(CurrentCube + " !#@$@1$!");
                //**************************************
                //Set character pair to current cube in tower
                SetCamera(CurrentCube);

                StartCoroutine(ProgressionMoveUp());

                CubeEnd = false;

            }
            //any other scene
            else
            {
                CurrentCube = twrData.twrProgress;
                if (CurrentCube == twrData.twrCubeCount)
                    CurrentCube--;
                //Debug.Log("CAMERA   " + gameObject.name + " : "  + CurrentCube + " : : : " + TowerController.Instance.Grid.childCount);
                
                
                //***************************************
                //Set character pair to current cube in tower and Canvas
                SetCamera(CurrentCube);
               

            }


            //Check for endtower to enable exit trigger
            EndTowerCheck(CurrentCube);


        }
        else if (scene.buildIndex == 2)
        {
            if (twrData.index >= levelProgress)
            {
                Debug.Log(">>>>>" + CurrentTower);
                //If Tower is finished - increment levelProgress
                PlayerPrefs.SetInt("LevelProgress", CurrentTower + 1);
                levelProgress = PlayerPrefs.GetInt("LevelProgress", -1);
            }
        }
     
    }


    public void SetCamera(int curCubeTarget)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //Debug.Log("CURCUBE " + curCubeTarget + " : " + TowerController.Instance.Grid.childCount);
            if (TowerController.Instance.Grid.childCount != 0)
            {
                //Set camera to grid child
                TowerController.Instance.cameraHolder.position = TowerController.Instance.Grid.GetChild(curCubeTarget).position + Vector3.up * 0.6f;

                //Enable Canvas for currentCube
                TowerController.Instance.Grid.GetChild(curCubeTarget).GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator ProgressionMoveUp()
    {
        //SOME REWARD SEQUENCE YIELD ANOTHER COROUTINE
        Debug.Log("UPPPPP");
        yield return new WaitForSeconds(2f);

        CurrentCube++;

        yield return new WaitForEndOfFrame();

        if (TowerExit)
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
                tmpCont.FollowTarget.position = tmpCont.Grid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

                //Enable Canvas for currentCube
                tmpCont.Grid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);
            }


            Vector3 tmpPos = new Vector3(tmpCont.cameraHolder.position.x, tmpCont.FollowTarget.position.y, tmpCont.cameraHolder.position.z);
            //For camera
            StartCoroutine(tmpCont.StopLook(tmpCont.cameraHolder, tmpPos, 0.5f));
            //For elevator
            StartCoroutine(tmpCont.StopLook(tmpCont.elevatorHolder, new Vector3(tmpCont.elevatorHolder.position.x, tmpPos.y, tmpCont.elevatorHolder.position.z), 1.5f));

           
        }

        

      

    }

    public IEnumerator ProgressionMoveRight()
    {
        if(TowerExit)
        {
            TowerExit = false;
            //SOME REWARD SEQUENCE YIELD ANOTHER COROUTINE
            Debug.Log("RIGHTTTTTT");
            yield return new WaitForSeconds(2f);

            LevelsController.Instance.MoveToNextTower(levelProgress);
        }
      

    }

    public IEnumerator TowerEndSequence ()
    {
        TowerController towerCont = TowerController.Instance;

        //Move elevator upstairs
        //Set follow object to null
        towerCont.vcam.m_LookAt = null;
        towerCont.FollowTarget.position = towerCont.Grid.parent.GetChild(1).position;
        //Enable Canvas for currentCube
        towerCont.Grid.GetChild(CurrentCube - 1).GetChild(1).gameObject.SetActive(false);

        Vector3 tmpPos = new Vector3(towerCont.cameraHolder.position.x, towerCont.FollowTarget.position.y, towerCont.cameraHolder.position.z);

        //For elevator
        StartCoroutine(towerCont.StopLook(towerCont.elevatorHolder, new Vector3(towerCont.elevatorHolder.position.x, tmpPos.y, towerCont.elevatorHolder.position.z), 3f));

        yield return new WaitForSecondsRealtime(2f);

        //Move Camera trigger in elevator pair to top, enable endtower camera 
        towerCont.vcamTowerEnd.gameObject.SetActive(true);
        //Disable vcam
        towerCont.vcam.gameObject.SetActive(false);

    }


    //Generate tower cubes
    public void RandomizeTower()
    {
        twrData = new TowerData();
        twrData.index = towerIndex;
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
