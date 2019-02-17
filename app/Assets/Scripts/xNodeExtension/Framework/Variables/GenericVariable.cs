using System;

public class GenericVariable<T> {
    public string variableName;

    public T value;
    public T defaultValue;

    public void SetValue(T value){
        this.value = value;
    }

    public T GetValue(){
        return value;
    }

    public void Reset(){
        this.value = this.defaultValue;
    }

    public string GetKey(){
        return this.variableName;
    }

    public void SetKey(string key){
        this.variableName = key;
    }

    public Type GetVariableType(){
        return typeof(T);
    }
}