using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public class iCS_FileSpecAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string iCanScriptFile {
        get { return myICanScriptFile; }
        set { myICanScriptFile= value; }
    }
    private string myICanScriptFile= null;

    public string iCanScriptFileGUID {
        get { return myICanScriptFileGUID; }
        set { myICanScriptFileGUID= value; }
    }
    private string myICanScriptFileGUID= null;
    
    // ======================================================================
    public override string ToString() { return "iCS_FileSpec"; }
}
