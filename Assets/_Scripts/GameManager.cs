using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Material[] materials;
    public Camera camHolder;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GrabRayObj();
        }
    }



    public void GrabRayObj()
    {
        RaycastHit hit;
        Ray ray = camHolder.ScreenPointToRay(Input.mousePosition);

        Debug.DrawLine(camHolder.transform.position, ray.direction, Color.red, 5f);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.CompareTag("Tile"))
            {
                objectHit.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
            }

        }
    }

}
