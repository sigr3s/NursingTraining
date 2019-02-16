

using System;
using XNode;

namespace NT.Atributes{

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NTInputAttribute : Node.InputAttribute {
        public NTInputAttribute(    Node.ShowBackingValue backingValue = Node.ShowBackingValue.Unconnected, 
                                    Node.ConnectionType connectionType = Node.ConnectionType.Override,
                                    Node.TypeConstraint typeConstraint = Node.TypeConstraint.Strict, 
                                    bool instancePortList = false) {
            this.backingValue = backingValue;
            this.connectionType = connectionType;
            this.instancePortList = instancePortList;
            this.typeConstraint = typeConstraint;
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class NTOutputAttribute : Node.OutputAttribute {
         public NTOutputAttribute(  Node.ShowBackingValue backingValue = Node.ShowBackingValue.Never, 
                                    Node.ConnectionType connectionType = Node.ConnectionType.Multiple, 
                                    bool instancePortList = false) {
            this.backingValue = backingValue;
            this.connectionType = connectionType;
            this.instancePortList = instancePortList;
        }
    }

}