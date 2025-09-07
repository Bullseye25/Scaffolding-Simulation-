using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationSupport : MonoBehaviour
{ 
    /// <summary>
  /// Reloads the currently active scene, resetting everything back to its start state.
  /// </summary>
    public void ResetScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
