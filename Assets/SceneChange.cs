using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetButtonDown("SceneChange"))
        {
            ChangeScene("MainScene");
        }
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

