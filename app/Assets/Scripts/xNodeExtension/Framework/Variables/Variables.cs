
namespace NT.Variables
{
    [System.Serializable]
    public class StringVariable : GenericVariable<string>
    {
        public StringVariable(){

        }

        public StringVariable(string key, string value) : base(key, value)
        {
        }
    }

    [System.Serializable]
    public class FloatVariable : GenericVariable<float>
    {
        public FloatVariable(){

        }

        public FloatVariable(string key, float value) : base(key, value)
        {
        }
    }

    [System.Serializable]
    public class IntegerVariable : GenericVariable<int>
    {
        public IntegerVariable(){

        }
        
        public IntegerVariable(string key, int value) : base(key, value)
        {
        }
    }

    [System.Serializable]
    public class BooleanVariable : GenericVariable<bool>
    {
        public BooleanVariable(){

        }
        
        public BooleanVariable(string key, bool value) : base(key, value)
        {
        }
    }
}