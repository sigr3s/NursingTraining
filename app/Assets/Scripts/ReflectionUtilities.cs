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
        public static Dictionary<Type, List<string>> DesgloseInBasicTypes(Type t){
            FieldInfo[] fi = t.GetFields();

            Dictionary<Type, List<string>> desglosed = new Dictionary<Type, List<string>>();

            foreach(FieldInfo f in fi){
                if(IsBasicType(f.FieldType)){
                    List<string> variables;

                    if(desglosed.ContainsKey(f.FieldType)){
                        variables = desglosed[f.FieldType];
                        variables.Add(f.Name);
                        desglosed[f.FieldType] = variables;
                    }
                    else
                    {
                        variables = new List<string>();
                        variables.Add(f.Name);
                        desglosed.Add(f.FieldType, variables);
                    }
                }
                else
                {
                    Desglose(f.FieldType, ref desglosed, f.Name+ "/" );
                }
            }

            return desglosed;
        }

        public static void SetValueOf(ref object o, object value, List<string> path)
        {
            if(path.Count == 0){
                o = value;
            }
            else
            {
                if(o == null) return;
                Type objectType = o.GetType();

                FieldInfo[] fi = objectType.GetFields();

                foreach(FieldInfo f in fi){
                    if(path[0] == f.Name){
                        object fieldVal = f.GetValue(o);
                        path.RemoveAt(0);

                        SetValueOf(ref fieldVal, value, path);

                        f.SetValue(o, fieldVal);
                    }
                }
            }


        }

        private static void Desglose(Type t, ref Dictionary<Type, List<string>> deg, string root){
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
                    Desglose(f.FieldType, ref deg, root + f.Name+ "/" );
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
    

        public static object GetValueOf(List<string> path, object o){
            if(path.Count > 0){
                string current = path[0];

                Type t = o.GetType();

                FieldInfo fi = t.GetField(current);

                if(fi != null){
                    object fio = fi.GetValue(o);

                    path.RemoveAt(0);

                    return GetValueOf(path, fio);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return o;
            }
        }
    }
}