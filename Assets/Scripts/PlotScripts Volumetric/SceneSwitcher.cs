using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public int sceneID;

    public void switchSceneTo()
    {
        SceneManager.LoadSceneAsync(sceneID);
    }
}
