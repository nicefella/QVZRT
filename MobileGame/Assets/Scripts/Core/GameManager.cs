using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that persists across scene loads.
/// Stores the avatar data chosen in the creation screen.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AvatarData AvatarData = new AvatarData();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
