using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;

namespace NT.Nodes.Variables{

    [CustomNodeEditor(typeof(SetStringNode))]
    public class SetStringNodeEditor  : NTVariableNodeEditor<NTString> { }

    [CustomNodeEditor(typeof(SetFloatNode))]
    public class SetFloatNodeEditor  : NTVariableNodeEditor<NTFloat> { }

    [CustomNodeEditor(typeof(SetIntNode))]
    public class SetIntegerNodeEditor  : NTVariableNodeEditor<NTInt> { }

    [CustomNodeEditor(typeof(SetBoolNode))]
    public class SetBooleanNodeEditor  : NTVariableNodeEditor<NTBool> { }
}