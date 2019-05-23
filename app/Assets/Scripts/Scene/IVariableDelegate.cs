public interface IVariableDelegate
{
    object GetValue(string key);
    void SetValue(string key, object value);

    object GetUserVariable(string key);
    object SetUserVariable(string key);
}