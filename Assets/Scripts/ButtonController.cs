using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void StartButtonPress() {
        //start game
        SceneManager.LoadScene(1); //first level
    }

    public void WhooHooButtonPress() {
        //back to menu
        SceneManager.LoadScene(0);
        OverworldManager.Instance.nextCutscene = 0;
    }
}
