using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public CityConfig config { get; private set; }

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadConfig();
    }

    private void LoadConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "game_config.json");
        string configJson = File.ReadAllText(configPath);
        config = JsonUtility.FromJson<CityConfig>(configJson);
    }
}
