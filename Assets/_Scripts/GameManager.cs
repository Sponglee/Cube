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
            GameObject tmpObj = GrabRayObj();

            if (tmpObj.CompareTag("Tile"))
            {
                tmpObj.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            GameObject tmpObj = GrabRayObj();
            if (tmpObj.CompareTag("Tile"))
            {
                tmpObj.transform.parent.parent.parent.parent.GetComponent<Animator>().SetTrigger("Open");
            }
        }
    }



    public GameObject GrabRayObj()
    {
        RaycastHit hit;
        Ray ray = camHolder.ScreenPointToRay(Input.mousePosition);

        Debug.DrawLine(camHolder.transform.position, ray.direction, Color.red, 5f);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHit = hit.transform.gameObject;
            return objectHit;

        }
        return null;
    }

}
