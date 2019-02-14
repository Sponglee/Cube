using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{

    public Transform startPoint;
    public Transform endPoint;
    public float speed = 1;

    public CubeController cube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cube.CubeOpened)
        {
            transform.gameObject.SetActive(false);
        }

        if (transform.position != new Vector3(endPoint.position.x, 0.3f, endPoint.position.z))
        {
           
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(endPoint.position.x, 0.3f, endPoint.position.z), speed*0.001f);
        }
        else 
        {
           
            Transform tmp = startPoint;
            startPoint = endPoint;
            endPoint = tmp;
        }
        
    }
}
