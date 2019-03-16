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

    //Current tower index
    public int towerIndex = 0;

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

            CurrentCube = twrData.twrProgress;
            
        }


    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 2)
        {
            if(CubeEnd)
            {
                CurrentCube++;
            }
            else
            {
                CurrentCube = twrData.twrProgress;
            }
            



            if(CurrentCube >= twrData.levels.Count)
            {
                SceneManager.LoadScene("TowerExmpl");
            }



        }
    }



   

    //Generate tower cubes
    public void RandomizeTower()
    {
        twrData = new TowerData();
        twrData.levels = new List<CubeData>();
        twrData.twrProgress = 0;


        foreach (Transform gridCube in TowerController.Instance.TowerGrid)
        {
           

            cubeInfo = new CubeData();
            cubeInfo.type = 3;

            //For each side (4 - right , back, left, front)
            for (int i = 0; i < cubeInfo.sides.Length; i++)
            {
                //Debug.Log("once");
                //Set and remember random color per side
                int randomMat = UnityEngine.Random.Range(1, materials.Length);

                //Remember color for a side 
                cubeInfo.sides[i].sideMat = randomMat;
                cubeInfo.sides[i].elemColors = new int[cubeInfo.type * cubeInfo.type];

                for (int j = 0; j < cubeInfo.type * cubeInfo.type; j++)
                {
                    //Randomize side's elem colorss
                    float matRandomizer = UnityEngine.Random.Range(0, 100);

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
            twrData.levels.Add(cubeInfo);


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
