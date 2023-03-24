using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.SimulationControlUI
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

