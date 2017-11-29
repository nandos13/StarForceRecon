using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

    IEnumerator WaitandLoad()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(17);
        SceneManager.LoadScene("Menu");
        //print("WaitAndPrint " + Time.time);
    }

    IEnumerator Start()
    {
        //print("Starting " + Time.time);

        // Start function WaitAndPrint as a coroutine
        yield return StartCoroutine("WaitandLoad");
        //print("Done " + Time.time);
    }
}