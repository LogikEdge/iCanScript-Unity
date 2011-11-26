using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
public class UK_InPortAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Name {
        get { return myName; }
        set { myName= value; }
    }
    private string myName= null;

    public Type ElementType {
        get { return myElementType; }
        set { myElementType= value; }
    }
    private Type myElementType= null;

    // ======================================================================
    public override string ToString() { return "UK_InPort"; }
}
