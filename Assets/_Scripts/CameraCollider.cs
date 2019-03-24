using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCollider : MonoBehaviour
{
    public int sceneIndex;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COINK " + other.name);
        if (sceneIndex == 1)
        {
            if (other.CompareTag("Door"))
            {
                FadeCanvas.Instance.FadeOut(1f, Color.white);   

                StartCoroutine(StopLoadTransition("Main", 1f));
            }
            else if (other.gameObject.CompareTag("Finish"))
            {
                TowerController.Instance.StopAllCoroutines();
                //FadeCanvas.Instance.FadeOut(0.05f,Color.black);
                StartCoroutine(StopLoadTransition("Levels", 0.05f));
                ProgressManager.Instance.TowerExit = true;
            }
        }
        else if (sceneIndex == 2)
        {
             //Top part of tower
            if (other.gameObject.CompareTag("Door"))
            {
                StartCoroutine(ProgressManager.Instance.ProgressionMoveRight());
            }
        }

    }


    public IEnumerator StopLoadTransition(string scene, float time)
    {

        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(scene);
    }

}
