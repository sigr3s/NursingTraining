using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;

namespace NT.Nodes.Variables{

    [CustomNodeEditor(typeof(StringNode))]
    public class StringNodeEditor  : NTVariableNodeEditor<NTString> { }

    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor  : NTVariableNodeEditor<NTFloat> { }

    [CustomNodeEditor(typeof(IntegerNode))]
    public class IntegerNodeEditor  : NTVariableNodeEditor<NTInt> { }

    [CustomNodeEditor(typeof(BoolNode))]
    public class BooleanNodeEditor  : NTVariableNodeEditor<NTBool> { }

    [CustomNodeEditor(typeof(SurgeonNode))]
    public class SurgeonNodeEditor : NTVariableNodeEditor<NTSurgeon> {}
}