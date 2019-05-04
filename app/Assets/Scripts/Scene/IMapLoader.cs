using System.Collections.Generic;

public interface IMapLoader
{
    void LoadMap(Dictionary<string, SceneGameObject> loadedData);
}