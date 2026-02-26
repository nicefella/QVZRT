using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that persists across all scenes.
/// Owns AvatarData so customisation choices survive scene transitions.
///
/// Usage:
///   GameManager.Instance.AvatarData   — read/write avatar selections
///   GameManager.Instance.LoadScene("SceneName")  — scene transitions
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AvatarData AvatarData { get; private set; }

    private void Awake()
    {
        // Enforce singleton — destroy any duplicate that arrives in a new scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AvatarData = new AvatarData();
    }

    /// <summary>
    /// Load a scene by name. Use the FadeScreen component on individual
    /// canvases to add a black-fade transition before calling this.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
