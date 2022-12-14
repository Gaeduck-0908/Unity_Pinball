using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    IEnumerator gotoMain_a()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Main");
    }
    public void gotoMain()
    {
        StartCoroutine("gotoMain_a");
    }
}