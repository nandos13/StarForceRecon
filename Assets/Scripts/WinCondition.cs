using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Terrible win condition so the game can be completed. */
public class WinCondition : MonoBehaviour
{
    public static int nodesLeft = 0;

    private static WinCondition instance;
    public Canvas WinCanvasPrefab;

    private void Awake()
    {
        instance = this;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        nodesLeft = 0;
        instance = null;
    }

    public static void IncrementNodeCounter()
    {
        nodesLeft++;
    }

    public static void DecrementNodeCounter()
    {
        nodesLeft--;

        if (instance == null)
            return;

        if (nodesLeft == 0)
        {
            // Win the game
            Time.timeScale = 0;
            Debug.Log("Win");
            if (instance.WinCanvasPrefab != null)
            {
                Canvas canvas = GameObject.Instantiate(instance.WinCanvasPrefab);
                canvas.enabled = true;
                canvas.gameObject.SetActive(true);

                StarForceRecon.PlayerController.DisableCursor();
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
        }
    }
}
