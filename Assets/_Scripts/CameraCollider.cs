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
            FadeCanvas.Instance.FadeOut(1f);

            StartCoroutine(StopLoadTransition("Main", 1f));
        }
    }


    public IEnumerator StopLoadTransition(string scene, float time)
    {

        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(scene);
    }

}
