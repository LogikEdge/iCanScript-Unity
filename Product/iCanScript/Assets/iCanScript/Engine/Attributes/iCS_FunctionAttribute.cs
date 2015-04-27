using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class iCS_FunctionAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Name {
        get { return myName; }
        set { myName= value; }
    }
    private string myName= null;
    
    public string Return {
        get { return myReturn; }
        set { myReturn= value; }
    }
    private string myReturn= null;
    
    public string Description {
        get { return myDescription; }
        set { myDescription= value; }
    }
    private string myDescription= null;
    
    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
	
	public string Obsolete {
		get { return myObsolete; }
		set { myObsolete= value; }
	}
	private string myObsolete= null;
    
    // ======================================================================
    public override string ToString() { return "iCS_Function"; }
}
