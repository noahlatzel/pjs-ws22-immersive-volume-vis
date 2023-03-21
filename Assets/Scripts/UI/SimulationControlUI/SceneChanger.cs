using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    /*public void Update()
    {
        if (Input.GetButtonDown("SceneChange"))
        {
            ChangeScene("PlotScene");
        }
    }*/
    
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

