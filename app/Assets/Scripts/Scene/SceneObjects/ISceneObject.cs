using System;
using System.Collections.Generic;

namespace NT
{
    public interface ISceneObject {
        List<Type> GetCompatibleNodes();
        List<string> GetCallbacks();
    }
}