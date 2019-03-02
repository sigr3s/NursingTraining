using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NT.Variables;

namespace NT
{
    public static class ReflectionUtilities {
        public static Type[] nodeTypes { get { return _nodeTypes != null ? _nodeTypes : _nodeTypes = GetNodeTypes(); } }
        public static Type[] variableNodeTypes { get { return _variableNodeTypes != null ? _variableNodeTypes : _variableNodeTypes = GetVariableNodeTypes(); } }

        [NonSerialized] private static Type[] _nodeTypes = null;
        [NonSerialized] private static Type[] _variableNodeTypes = null;

        public static Type[] GetNodeTypes() {
            return GetDerivedTypes(typeof(XNode.Node));
        }

        public static Type[] GetVariableNodeTypes() {
            return GetDerivedTypes(typeof(INTVaribale));
        }

        public static Type[] GetDerivedTypes(Type baseType) {
            List<System.Type> types = new List<System.Type>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                types.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
            }
            return types.ToArray();
        }
    }
}