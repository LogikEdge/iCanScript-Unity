using UnityEngine;
using System.Collections;

public class WD_DynamicMenu {
    enum MenuStateEnum { Idle, Company, Package, Function }
    
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    MenuStateEnum   CurrentState= MenuStateEnum.Idle;
    string          SelectedCompany= null;
    string          SelectedPackage= null;
    string          SelectedFunction= null;
    Vector2         MenuPosition= Vector2.zero;
    int             Selection   = -1;
    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public bool IsActive { get { return CurrentState != MenuStateEnum.Idle; }}

	// ----------------------------------------------------------------------
    void Reset() {
        CurrentState= MenuStateEnum.Idle;
        SelectedCompany= null;
        SelectedPackage= null;
        SelectedFunction= null;
        MenuPosition= Vector2.zero;
        Selection= -1;
    }
    
	// ----------------------------------------------------------------------
    // Activate the dynamic menu if it is not already active.
    public void Activate(Vector2 mouseDownPosition) {
        if(!IsActive) CurrentState= MenuStateEnum.Company;
    }
    
	// ----------------------------------------------------------------------
    public void Update(WD_EditorObject selectedObject, WD_EditorObjectMgr editorObjects, Vector2 mouseDownPosition) {
        // Update mouse position if not already done.
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;

        // Nothing to show if menu is inactive.
        if(MenuPosition != mouseDownPosition || !IsActive) {
            Reset();
            return;
        }

        // Process the menu state.
        switch(CurrentState) {
            case MenuStateEnum.Idle:
                break;
            case MenuStateEnum.Company:
                UpdateCompany();
                break;
            case MenuStateEnum.Package:
                UpdatePackage();
                break;
            case MenuStateEnum.Function:
                UpdateFunction(selectedObject, editorObjects);
                break;
        }
    }

	// ----------------------------------------------------------------------
    void UpdateCompany() {
        string[] companies= WD_DataBase.GetCompanies();
        float width= 0;
        float height= 0;
        foreach(var company in companies) {
            Vector2 size= GUI.skin.button.CalcSize(new GUIContent(company));
            if(size.x > width) width= size.x;
            if(size.y > height) height= size.y;
        }
        Selection= GUI.SelectionGrid(new Rect(MenuPosition.x,MenuPosition.y,width,height*companies.Length), Selection, companies, 1);
        if(Selection != -1) {
            SelectedCompany= companies[Selection];
            CurrentState= MenuStateEnum.Package;
            Selection= -1;
        }        
    }
	// ----------------------------------------------------------------------
    void UpdatePackage() {
        string[] packages= WD_DataBase.GetPackages(SelectedCompany);
        float width= 0;
        float height= 0;
        foreach(var package in packages) {
            Vector2 size= GUI.skin.button.CalcSize(new GUIContent(package));
            if(size.x > width) width= size.x;
            if(size.y > height) height= size.y;
        }
        Selection= GUI.SelectionGrid(new Rect(MenuPosition.x,MenuPosition.y,width,height*packages.Length), Selection, packages, 1);
        if(Selection != -1) {
            SelectedPackage= packages[Selection];
            CurrentState= MenuStateEnum.Function;
            Selection= -1;
        }                
    }
	// ----------------------------------------------------------------------
    void UpdateFunction(WD_EditorObject selectedObject, WD_EditorObjectMgr editorObjects) {
        // Gather all functions.
        string[] conversions= WD_DataBase.GetConversions(SelectedCompany, SelectedPackage);
        string[] functions  = WD_DataBase.GetFunctions(SelectedCompany, SelectedPackage);
        string[] classes    = WD_DataBase.GetClasses(SelectedCompany, SelectedPackage);
        string[] all= new string[conversions.Length+functions.Length+classes.Length];
        conversions.CopyTo(all,0);
        functions.CopyTo  (all, conversions.Length);
        classes.CopyTo    (all, conversions.Length+functions.Length);

        float width= 0;
        float height= 0;
        foreach(var func in all) {
            Vector2 size= GUI.skin.button.CalcSize(new GUIContent(func));
            if(size.x > width) width= size.x;
            if(size.y > height) height= size.y;
        }
        Selection= GUI.SelectionGrid(new Rect(MenuPosition.x,MenuPosition.y,width,height*all.Length), Selection, all, 1);
        if(Selection != -1) {
            SelectedFunction= all[Selection];
            CurrentState= MenuStateEnum.Idle;
            Selection= -1;
            Debug.Log("Selected: "+SelectedCompany+":"+SelectedPackage+":"+SelectedFunction);
            WD_BaseDesc desc= WD_DataBase.GetDescriptor(SelectedCompany, SelectedPackage, SelectedFunction);
            if(desc == null) {
                Debug.LogError("Unable to find: "+SelectedCompany+":"+SelectedPackage+":"+SelectedFunction+" in Database !!!");
            };
            desc.CreateInstance(editorObjects, (selectedObject != null ? selectedObject.InstanceId : -1), MenuPosition);
        }                

    }
}
