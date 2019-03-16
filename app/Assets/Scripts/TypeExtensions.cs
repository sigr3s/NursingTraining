using System;
using UnityEngine;
 
public static class TypeExtensions
{
    public static bool IsNumber(this Type t){
        return (t == typeof(float)) || (t == typeof(int)) || (t == typeof(double));
    }

    public static bool IsString(this Type t){
        return (t == typeof(string));
    }

    public static bool IsBool(this Type t){
        return (t == typeof(bool));
    }
}