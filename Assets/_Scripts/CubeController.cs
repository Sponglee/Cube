using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    //Material pool
    public Material[] materials;
    //Sides references
    public Transform[] cubeSides;
    public Transform cuveBottom;

    //Camera reference points
    public Transform[] cameraPoints;
    public int activeCameraPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize cube sides
        foreach (Transform cubeSide in cubeSides)
        {
            Material tmpMat = materials[Random.Range(0, materials.Length)];
            //Grab references to each element per side
            foreach (Transform child in cubeSide.GetChild(0).GetChild(0))
            {
                //Randomize side's elem colorss
                float matRandomizer = Random.Range(0, 100);
                if (matRandomizer <= 50)
                {
                    //Side color elem
                    child.GetComponent<Renderer>().material = tmpMat;
                }
                else
                {
                    //Blank elem
                    child.GetComponent<Renderer>().material = materials[0];
                }
            }
        }
    }

    private void Update()
    {
       
    }


    public void MoveCamera(Transform camera)
    {
        activeCameraPoint++;
        if(activeCameraPoint>= cameraPoints.Length)
        {
            activeCameraPoint = 0;
        }
        camera.SetParent(cameraPoints[activeCameraPoint]);
        StartCoroutine(StopCamera(camera));
    }

    public float cameraSpeed = 1f;
    public IEnumerator StopCamera(Transform camera)
    {
        //Smoothly move camera to point
        while (camera.transform.localPosition != new Vector3(0f,0f,0f))
        {
            camera.localPosition = Vector3.Lerp(camera.transform.localPosition, Vector3.zero, cameraSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("REEE");
    }

}
