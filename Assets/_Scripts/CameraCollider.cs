using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            FadeCanvas.Instance.FadeOut(1f, Color.white);

            StartCoroutine(StopLoadTransition("Main", 1f));
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            TowerController.Instance.StopAllCoroutines();
            //FadeCanvas.Instance.FadeOut(0.05f,Color.black);
            StartCoroutine(StopLoadTransition("Levels", 0.05f));

        }
    }


    public IEnumerator StopLoadTransition(string scene, float time)
    {

        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(scene);
    }

}
