using UnityEngine;
using System.Collections;

public class WD_DynamicMenu {
    enum MenuStateEnum { Idle, Company, Package, Function }
    
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    Vector2         MenuPosition= Vector2.zero;
    int             Selection   = -1;
    MenuStateEnum   CurrentState= MenuStateEnum.Idle;
    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public bool IsActive { get { return CurrentState != MenuStateEnum.Idle; }}

	// ----------------------------------------------------------------------
    void Reset() {
        CurrentState= MenuStateEnum.Idle;
        MenuPosition= Vector2.zero;
        Selection= -1;
    }
    
	// ----------------------------------------------------------------------
    public void Activate(Vector2 mouseDownPosition) {
        CurrentState= MenuStateEnum.Company;
    }
    
	// ----------------------------------------------------------------------
    public void Update(Vector2 mouseDownPosition) {
        // Nothing to show if menu is inactive.
        if(!IsActive) { Reset(); return; }

        // 
        string[] companies= WD_DataBase.GetCompanies();
        float width= 0;
        float height= 0;
        foreach(var company in companies) {
            Vector2 size= GUI.skin.button.CalcSize(new GUIContent(company));
            if(size.x > width) width= size.x;
            if(size.y > height) height= size.y;
        }
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;
        Selection= GUI.SelectionGrid(new Rect(MenuPosition.x,MenuPosition.y,width,height*companies.Length), Selection, companies, 1);
        if(Selection != -1 || MenuPosition != mouseDownPosition) {
            if(Selection != -1) Debug.Log("Selection was changed to "+companies[Selection]);        
            Reset();
        }
    }
}
