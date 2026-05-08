using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public string sceneName;

   
    public float delay = 0f;

    public void ChangeScene()
    {
        if (delay <= 0f)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Invoke(nameof(LoadSceneNow), delay);
        }
    }

    void LoadSceneNow()
    {
        SceneManager.LoadScene(sceneName);
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}