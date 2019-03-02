

using System;
using System.Collections.Generic;
using NT.Variables;

namespace NT.SceneObjects
{
    public class NTSceneObject<T> : NTVariable<T>, ISceneObject
    {
        public List<string> GetCallbacks()
        {
            throw new NotImplementedException();
        }

        public List<Type> GetCompatibleNodes()
        {
            throw new NotImplementedException();
        }
    }
}