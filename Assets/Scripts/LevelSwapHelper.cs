using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// My static class made it where the reference to it on the Timeline was destroyed when swapping scenes. This works around that.
/// </summary>
public class LevelSwapHelper : MonoBehaviour
{
    public void LoadLevel(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
