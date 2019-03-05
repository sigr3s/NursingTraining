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
            return GetDerivedTypes(typeof(NTVariable));
        }

        public static Type[] GetDerivedTypes(Type baseType) {
            List<System.Type> types = new List<System.Type>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                types.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
            }
            return types.ToArray();
        }

        /// Dict<Type, List<string>>
        /// 
        public static Dictionary<Type, List<string>> DeglosseInBasicTypes(Type t){
            FieldInfo[] fi = t.GetFields();

            Dictionary<Type, List<string>> desglossed = new Dictionary<Type, List<string>>();

            foreach(FieldInfo f in fi){
                if(IsBasicType(f.FieldType)){
                    List<string> variables;

                    if(desglossed.ContainsKey(f.FieldType)){
                        variables = desglossed[f.FieldType];
                        variables.Add(f.Name);
                        desglossed[f.FieldType] = variables;
                    }
                    else
                    {
                        variables = new List<string>();
                        variables.Add(f.Name);
                        desglossed.Add(f.FieldType, variables);
                    }
                }
                else
                {
                    Deglosse(f.FieldType, ref desglossed, f.Name+ "/" );
                }
            }

            return desglossed;
        }

        private static void Deglosse(Type t, ref Dictionary<Type, List<string>> deg, string root){
            FieldInfo[] fi = t.GetFields();

            foreach(FieldInfo f in fi){
                if(IsBasicType(f.FieldType)){
                    List<string> variables;

                    if(deg.ContainsKey(f.FieldType)){
                        variables = deg[f.FieldType];
                        variables.Add(root + f.Name);
                        deg[f.FieldType] = variables;
                    }
                    else
                    {
                        variables = new List<string>();
                        variables.Add(root + f.Name);
                        deg.Add(f.FieldType, variables);                        
                    }
                }
                else
                {
                    Deglosse(f.FieldType, ref deg, root + f.Name+ "/" );
                }
            }

        }

        public static bool IsBasicType(Type t){
            bool isBasic = (t == typeof(string) || 
                            t == typeof(int)    ||
                            t == typeof(float)  ||
                            t == typeof(double) ||
                            t == typeof(bool) );

            return isBasic;
        }
    }
}