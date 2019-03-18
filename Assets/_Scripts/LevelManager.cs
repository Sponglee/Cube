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
               
                //PlayerPrefs.SetInt(string.Format("Tower{0}_Current",towerIndex), value);
                if (twrData != null)
                {
                    twrData.twrProgress = value;
                    Debug.Log(twrData.twrProgress + " : :: : " + twrData.twrCubeCount);
                    //Enable Next Tower Bool
                    if (twrData.twrProgress >= twrData.twrCubeCount)
                    {
                        TowerEnd = true;
                       
                    }

                }



                SaveSystem.SaveLevel(towerIndex, twrData);

            }
            else
            {
                currentCube = value;
            }

        }
    }
   





    // Start is called before the first frame update
    void Awake()
    {

        //Make sure only 1 isntance is up
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }





        //Load tower data, if there's none - randomize it
        twrData = SaveSystem.LoadLevel(towerIndex);

        if (twrData == null)
        {
            //Load backup 
            //*** HERE ***
            RandomizeTower();
            CurrentCube = 0;
            


        }
        else
        {
           
            Debug.Log("CurrentCubeLOAD");
            CurrentCube = twrData.twrProgress;
            if(CurrentCube>twrData.twrCubeCount)
            {
                CurrentCube = twrData.twrCubeCount;
            }
        }



        //**************************************

        //Initialize tower
        TowerController.Instance.InitializeTower(twrData);

        //Set character pair to current cube in tower
        TowerController.Instance.cameraHolder.position = TowerController.Instance.TowerGrid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

        //Enable Canvas for currentCube
        TowerController.Instance.TowerGrid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);


    }

    //When Scene is loaded (only after another scene with levelManager preset, not first time)
    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("RE");
       
        if (level == 2)
        {
            //TowerController.Instance.InitializeTower(twrData);

            if (CubeEnd)
            {



                //Vector3 tmpPos = new Vector3(cameraHolder.position.x, FollowTarget.position.y, cameraHolder.position.z);

                ////For camera
                //StartCoroutine(StopLook(cameraHolder, tmpPos, 0.2f));
                ////For elevator
                //StartCoroutine(StopLook(elevatorHolder, new Vector3(elevatorHolder.position.x, tmpPos.y, elevatorHolder.position.z), 1f));


                //**************************************
                //Set character pair to current cube in tower
                TowerController.Instance.cameraHolder.position = TowerController.Instance.TowerGrid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

                //Enable Canvas for currentCube
                TowerController.Instance.TowerGrid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);

                StartCoroutine(ProgressionMoveUp());
                
                CubeEnd = false;

            }
            else
            {
                CurrentCube = twrData.twrProgress;

                //***************************************
                //Set character pair to current cube in tower
                TowerController.Instance.cameraHolder.position = TowerController.Instance.TowerGrid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

                //Enable Canvas for currentCube
                TowerController.Instance.TowerGrid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);

            }



          


        }
    }

    public IEnumerator ProgressionMoveUp()
    {
        yield return new WaitForSeconds(2f);

        CurrentCube++;

        yield return new WaitForEndOfFrame();

        if (TowerEnd)
        {
            SceneManager.LoadScene("TowerExmpl");
            TowerEnd = false;
        }
        else
        {


            TowerController tmpCont = TowerController.Instance;
            //***************************************
            //Set character pair to current cube in tower
            tmpCont.FollowTarget.position = tmpCont.TowerGrid.GetChild(CurrentCube).position + Vector3.up * 0.6f;

            //Enable Canvas for currentCube
            tmpCont.TowerGrid.GetChild(CurrentCube).GetChild(1).gameObject.SetActive(true);


            Vector3 tmpPos = new Vector3(tmpCont.cameraHolder.position.x, tmpCont.FollowTarget.position.y, tmpCont.cameraHolder.position.z);

            //For camera
            StartCoroutine(tmpCont.StopLook(tmpCont.cameraHolder, tmpPos, 0.5f));
            //For elevator
            StartCoroutine(tmpCont.StopLook(tmpCont.elevatorHolder, new Vector3(tmpCont.elevatorHolder.position.x, tmpPos.y, tmpCont.elevatorHolder.position.z), 1.5f));

           
        }

        

      

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
      
        
       
        SaveSystem.SaveLevel(towerIndex, twrData);

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
