using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Material bottomMaterial;

    //Cinemachine camera holder
    public Transform camHolder;
    //Physical Camera ref for rays etc
    public Camera physicalCam;
    //Active cube ref
    public CubeController activeCube;

    // Start is called before the first frame update
    void Start()
    {
        //Debug Initialize camera
        ChangeCameraState(activeCube.cameraPoints[activeCube.activeCameraPoint]);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Click on the bottom cube to select
            GameObject tmpObj = GrabRayObj();
            if (tmpObj.CompareTag("Tile"))
            {
                BottomCheck(tmpObj.transform, tmpObj);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Switch between camera points
            activeCube.MoveCamera(camHolder);
        }
        else if(Input.GetMouseButtonDown(2))
        {
            //Debug open cube anim trigger
            GameObject tmpObj = GrabRayObj();
            if (tmpObj.CompareTag("Tile"))
            {
                tmpObj.transform.parent.parent.parent.parent.GetComponent<Animator>().SetTrigger("Open");
            }
        }
    }


    //Check if elem is ref to other sides
    public void BottomCheck(Transform tile, GameObject tmpObj)
    {
        CubeElemController tmpTile = tile.GetComponent<CubeElemController>();
        if (tmpTile.BottomLinks.Count != 0)
        {
            tmpObj.GetComponent<Renderer>().material = tmpTile.BottomLinks[0].transform.GetComponent<Renderer>().material;
            Debug.Log(tmpTile.BottomLinks[0].transform.GetComponent<Renderer>().material);
        }


    }



    //Reset camera transform and set Parent
    public void ChangeCameraState(Transform cameraParent)
    {
        
        camHolder.SetParent(cameraParent);
        camHolder.GetComponent<CinemachineVirtualCamera>().m_LookAt = activeCube.transform;
        camHolder.localRotation = Quaternion.identity;
        camHolder.localPosition = Vector3.zero;
        camHolder.localScale = Vector3.one;
    }

    public GameObject GrabRayObj()
    {
        RaycastHit hit;
        Ray ray = physicalCam.ScreenPointToRay(Input.mousePosition);

        Debug.DrawLine(camHolder.transform.position, ray.direction, Color.red, 5f);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHit = hit.transform.gameObject;
            return objectHit;

        }
        return null;
    }

}
