using System;
using System.Collections.Generic;

namespace NT.Variables
{
    [Serializable]
    public class StringVariables : GenericVariables<StringVariable, string>{
    
    }

    [Serializable]
    public class FloatVariables : GenericVariables<FloatVariable, float>{

    }

    [Serializable]
    public class IntegerVariables : GenericVariables<IntegerVariable, int>{

    }

    [Serializable]
    public class BooleanVariables : GenericVariables<BooleanVariable, bool>{

    }
    
}