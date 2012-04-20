using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassListController : DSTableViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSTableView                 myTableView      = null;
    List<iCS_ReflectionDesc>    myClasses        = null;
	List<string>				myPackages       = new List<string>();
	List<string>				myCompanies		 = new List<string>();
    List<iCS_ReflectionDesc>    myFilteredClasses= null;
    GUIStyle                    myColumnDataStyle= null;

    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int     kSpacer         = 8;
    const int     kMarginSize     = 10;
	const string  kClassColumnId  = "Class";
	const string  kPackageColumnId= "Package";
	const string  kCompanyColumnId= "Company";

    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public DSView View { get { return myTableView; }}
    public GUIStyle ColumnDataStyle {
        get { return myColumnDataStyle; }
        set {
            myColumnDataStyle= value;
        }
    }
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
	public iCS_ClassListController() {
        // Default GUI Style.
        if(myColumnDataStyle == null) myColumnDataStyle= EditorStyles.label;
        
        // Get list all classes.
        myClasses= iCS_DataBase.GetClasses();
		foreach(var desc in myClasses) {
			if(!myPackages.Contains(desc.Package)) myPackages.Add(desc.Package);
			if(!myCompanies.Contains(desc.Company)) myCompanies.Add(desc.Company);
		}
        myFilteredClasses= myClasses;
        
        // Build table view.
        myTableView= new DSTableView(new RectOffset(kSpacer,kSpacer,0,kSpacer), true, new GUIContent("Classes"), DSView.AnchorEnum.Center, true, true);
        DSTableColumn classColumn= new DSTableColumn(kClassColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Class"), DSView.AnchorEnum.CenterLeft);
        DSTableColumn packageColumn= new DSTableColumn(kPackageColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Package"), DSView.AnchorEnum.CenterLeft);    
        DSTableColumn companyColumn= new DSTableColumn(kCompanyColumnId, new RectOffset(kSpacer,kSpacer,0,0), new GUIContent("Company"), DSView.AnchorEnum.CenterLeft);
        myTableView.AddColumn(classColumn);
        myTableView.AddColumn(packageColumn);
        myTableView.AddColumn(companyColumn);  
        myTableView.DataSource= this;
    }
    // ---------------------------------------------------------------------------------
    public void Filter(string classSubstringFilter, string packageSubstringFilter, string companySubstringFilter) {
        myFilteredClasses= Prelude.filter(
            d=> {
				bool companyOk= true;
				if(!IsEmptyString(companySubstringFilter)) {
					if(d.Company.ToUpper().IndexOf(companySubstringFilter.ToUpper()) == -1) companyOk= false;
				}
				bool packageOk= true;
				if(!IsEmptyString(packageSubstringFilter)) {
					if(d.Package.ToUpper().IndexOf(packageSubstringFilter.ToUpper()) == -1) packageOk= false;
				}
				bool classOk= true;
				if(!IsEmptyString(classSubstringFilter)) {
					if(d.ClassType.Name.ToUpper().IndexOf(classSubstringFilter.ToUpper()) == -1) classOk= false;
				}
				return companyOk && packageOk && classOk;
			},
            myClasses);
    }
    public void Display(Rect frameArea) {
        myTableView.Display(frameArea);
    }
    
    // =================================================================================
    // TableViewDataSource
    // ---------------------------------------------------------------------------------
    public int NumberOfRowsInTableView(DSTableView tableView) {
        return myFilteredClasses.Count;
    }
    public Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
        iCS_ReflectionDesc desc= myFilteredClasses[row];
        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kClassColumnId) == 0) {
            return myColumnDataStyle.CalcSize(new GUIContent(desc.ClassType.Name));
        }
        if(string.Compare(columnId, kPackageColumnId) == 0) {
            return myColumnDataStyle.CalcSize(new GUIContent(desc.Package));
        }
        if(string.Compare(columnId, kCompanyColumnId) == 0) {
            return myColumnDataStyle.CalcSize(new GUIContent(desc.Company));
        }
        return Vector2.zero;
    }
    public void DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect position) {
        iCS_ReflectionDesc desc= myFilteredClasses[row];
        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kClassColumnId) == 0) {
            GUI.Label(position, desc.ClassType.Name, myColumnDataStyle);
        }
        if(string.Compare(columnId, kPackageColumnId) == 0) {
            GUI.Label(position, desc.Package, myColumnDataStyle);
        }
        if(string.Compare(columnId, kCompanyColumnId) == 0) {
            GUI.Label(position, desc.Company, myColumnDataStyle);
        }
    }
    // ---------------------------------------------------------------------------------
	public void OnMouseDown(DSTableView tableView, DSTableColumn tableColumn, int row) {}

    // =================================================================================
    // Miscelanious
    // ---------------------------------------------------------------------------------
	static bool IsEmptyString(string s) { return s == null || s == ""; }
}
