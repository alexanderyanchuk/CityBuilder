using System.IO;
using UnityEngine;
using Zenject;

public class ConfigInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<CityConfig>().FromMethod(() =>
        {
            string configPath = Path.Combine(Application.streamingAssetsPath, "game_config.json");
            string configJson = File.ReadAllText(configPath);
            return JsonUtility.FromJson<CityConfig>(configJson);
        }).AsSingle();
    }
}
