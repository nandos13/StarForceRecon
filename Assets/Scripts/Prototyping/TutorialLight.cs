using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLight : MonoBehaviour {

    public GameObject rep, jax, mer;

    public GameObject game, hudCanvas;

    public GameObject cam, camfake;
    public GameObject real, fake;

	void Rep()
    {
        rep.SetActive(true);
    }

    void Jax()
    {
        jax.SetActive(true);
    }

    void Mer()
    {
        mer.SetActive(true);
    }

    void Game()
    {
        cam.SetActive(true);
        real.SetActive(true);
        Destroy(fake);
        Destroy(camfake);
        game.SetActive(true);
        hudCanvas.SetActive(true);
    }
}
