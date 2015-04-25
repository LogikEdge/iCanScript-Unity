using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class iCS_ClassAttribute : Attribute {
    // ======================================================================
    // Optional Parameters
    // ----------------------------------------------------------------------
    public string Company {
        get { return myCompany; }
        set { myCompany= value; }
    }
    private string myCompany= null;

    public string Library {
        get { return myLibrary; }
        set { myLibrary= value; }
    }
    private string myLibrary= null;
    
    public string Description {
        get { return myDescription; }
        set { myDescription= value; }
    }
    private string myDescription= null;
    
    public string CompanyIcon {
        get { return myCompanyIcon; }
        set { myCompanyIcon= value; }
    }
    private string myCompanyIcon= null;

    public string Icon {
        get { return myIcon; }
        set { myIcon= value; }
    }
    private string myIcon= null;
    
    public bool BaseVisibility {
        get { return myBaseVisibility; }
        set { myBaseVisibility= value; }
    }
    private bool myBaseVisibility= false;
	
	public bool HideClassFromLibrary {
		get { return myHideClassFromLibrary; }
		set { myHideClassFromLibrary= value; }
	}
	private bool myHideClassFromLibrary= false;
    
    // ======================================================================
    public override string ToString() { return "iCS_Class"; }
}
