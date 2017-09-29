using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public string NextScene;

    public  void TransitionScene()
    {
        if(NextScene!=null)
            SceneManager.LoadScene(NextScene);
    }

    public void ExitGame()
    {
        print("Exiting...");
        Application.Quit();
    }
}
