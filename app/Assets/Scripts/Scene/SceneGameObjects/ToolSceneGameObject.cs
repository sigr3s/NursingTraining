using UnityEngine;

public class ToolSceneGameObject : SceneGameObject, ITool
{
    public Tools toolType;

    public Tools GetToolType()
    {
        return toolType;
    }
}