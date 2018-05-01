using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	//script to control all the different menus in the game

    public void StartClick()
    {
        SceneManager.LoadScene("LevelOne", LoadSceneMode.Single);
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }

    //sets another canvas to active and deactivates the original canvas
    public void TurnOnOtherCanvas(GameObject otherCanvas)
    {
		gameObject.SetActive (false);
		otherCanvas.SetActive (true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
