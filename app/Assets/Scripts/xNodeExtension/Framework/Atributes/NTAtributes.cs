

using System;
using XNode;

namespace NT.Atributes{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NTInputAttribute : Node.InputAttribute {
        public NTInputAttribute(    Node.ShowBackingValue backingValue = Node.ShowBackingValue.Unconnected, 
                                    Node.ConnectionType connectionType = Node.ConnectionType.Override,
                                    Node.TypeConstraint typeConstraint = Node.TypeConstraint.Strict, 
                                    bool instancePortList = false):
            base(backingValue, connectionType,typeConstraint, instancePortList){
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NTOutputAttribute : Node.OutputAttribute {
         public NTOutputAttribute(  Node.ShowBackingValue backingValue = Node.ShowBackingValue.Always, 
                                    Node.ConnectionType connectionType = Node.ConnectionType.Multiple, 
                                    bool instancePortList = false) : 
            base(backingValue, connectionType, instancePortList) {
        }
    }

}