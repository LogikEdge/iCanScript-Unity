using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassListController : DSTableViewDataSource {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSTableView             myTableView       = null;
    List<iCS_TypeInfo>      myClasses         = null;
	List<string>			myPackages        = new List<string>();
	List<string>			myCompanies		  = new List<string>();
    List<iCS_TypeInfo>      myFilteredClasses = null;
    GUIStyle                myColumnDataStyle = null;
    Action<Type>            myOnClassSelection= null;
    int                     mySelectedRow     = 0;

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
	public iCS_ClassListController(Action<Type> onClassSelection= null) {
        myOnClassSelection= onClassSelection;
        
        // Default GUI Style.
        if(myColumnDataStyle == null) myColumnDataStyle= EditorStyles.label;
        
        // Get list all classes.
        myClasses= iCS_LibraryDataBase.types;
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
		myTableView.SetSelection(classColumn, 0);
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
					if(d.DisplayName.ToUpper().IndexOf(classSubstringFilter.ToUpper()) == -1) classOk= false;
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
//		Debug.Log("#rows= "+myFilteredClasses.Count);
        return myFilteredClasses.Count;
    }
    public Vector2 LayoutSizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row) {
        iCS_TypeInfo desc= myFilteredClasses[row];
        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kClassColumnId) == 0) {
            return myColumnDataStyle.CalcSize(new GUIContent(desc.DisplayName));
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
        // Show selected row in reverse color.
        Color contentColor= GUI.contentColor;
        if(myOnClassSelection != null && row == mySelectedRow) {
            Color bkgColor= GUI.contentColor;
            bkgColor.r= 1f-bkgColor.r;
            bkgColor.g= 1f-bkgColor.g;
            bkgColor.b= 1f-bkgColor.b;
            GUI.contentColor= bkgColor;
        }
        iCS_TypeInfo desc= myFilteredClasses[row];
        string columnId= tableColumn.Identifier;
        if(string.Compare(columnId, kClassColumnId) == 0) {
            GUI.Label(position, desc.DisplayName, myColumnDataStyle);
        }
        if(string.Compare(columnId, kPackageColumnId) == 0) {
            GUI.Label(position, desc.Package, myColumnDataStyle);
        }
        if(string.Compare(columnId, kCompanyColumnId) == 0) {
            GUI.Label(position, desc.Company, myColumnDataStyle);
        }
        // Restore content color.
        if(myOnClassSelection != null && row == mySelectedRow) {
            GUI.contentColor= contentColor;
        }
    }
    // ---------------------------------------------------------------------------------
	public void OnMouseDown(DSTableView tableView, DSTableColumn tableColumn, int row) {
        mySelectedRow= row;
        if(myOnClassSelection != null) {
            iCS_TypeInfo desc= myFilteredClasses[row];
            myOnClassSelection(desc.CompilerType);            
        }
	}

    // =================================================================================
    // Miscelanious
    // ---------------------------------------------------------------------------------
	static bool IsEmptyString(string s) { return s == null || s == ""; }
}
