using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassModuleMenu : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject        Target          = null;
    Type                    ClassType       = null;
    bool                    IsCreateInstance= false;
    int                     ConstructorIdx  = 0;
    string[]                Constructors    = new String[1]{"None"};
    float                   MaxMethodWidth  = 0;
    iCS_ReflectionDesc[]    Fields          = null;
    iCS_ReflectionDesc[]    Properties      = null;
    iCS_ReflectionDesc[]    Methods         = null;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kMarginSize  = 10;
    const int   kScrollerSize= 16;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    int NbOfVariables { get { return Fields.Length + Properties.Length; }}
    int NbOfMethods   { get { return Methods.Length; }}
    
    // ---------------------------------------------------------------------------------
    public void Activate(iCS_EditorObject target, iCS_IStorage storage) {
        ClassType= target.RuntimeType;
        List<iCS_ReflectionDesc> fields= new List<iCS_ReflectionDesc>();
        List<iCS_ReflectionDesc> properties= new List<iCS_ReflectionDesc>();
        List<iCS_ReflectionDesc> methods= new List<iCS_ReflectionDesc>();
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(ClassType);
        foreach(var component in components) {
            if(component.IsField) {
                fields.Add(component);
            } else if(component.IsProperty) {
                properties.Add(component);
            } else {
                methods.Add(component);
                var signatureSize= EditorStyles.label.CalcSize(new GUIContent(component.FunctionSignatureNoThis));
                if(signatureSize.x+12f > MaxMethodWidth) {
                    MaxMethodWidth= signatureSize.x+12f;
                }
            }
        }
        Fields= fields.ToArray();
        Properties= properties.ToArray();
        Methods= methods.ToArray();
        Target= target;
    }
    
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        if(Target == null) return;
        
        EditorGUIUtility.LookLikeInspector();
        
        // Define GUI content.
        GUIContent applyButton     = new GUIContent("Apply");
        GUIContent cancelButton    = new GUIContent("Cancel");
        GUIContent constructorLabel= new GUIContent("Create Instance");
        GUIContent variableTitle   = new GUIContent("Variables");
        GUIContent methodTitle     = new GUIContent("Methods");
        
        // Compute content size.
        Vector2 labelSize= EditorStyles.label.CalcSize(constructorLabel);
        Vector2 variableTitleSize= EditorStyles.whiteLargeLabel.CalcSize(variableTitle);
        Vector2 methodTitleSize= EditorStyles.whiteLargeLabel.CalcSize(methodTitle);
        Vector2 applyButtonSize     = EditorStyles.radioButton.CalcSize(applyButton);
        Vector2 cancelButtonSize    = EditorStyles.radioButton.CalcSize(applyButton);
        float labelHeight= 4f+labelSize.y;
        float titleHeight= 4f+variableTitleSize.y;
        float buttonWidth= Mathf.Max(applyButtonSize.x, cancelButtonSize.x);
        float buttonHeight= 4f+Mathf.Max(applyButtonSize.y, cancelButtonSize.y);

        // Compute window parameters.
        float x     = kMarginSize;
        float y     = kMarginSize;
        float width = position.width-2f*kMarginSize;
        float height= position.height-2f*kMarginSize;
        float xMax  = x+width;
        float yMax  = y+height;
        
        // Compute header position.
        float headerHeight= 3f*labelSize.y+kMarginSize;
        Rect headerRect= new Rect(x, y, width, headerHeight);
        float remainingHeight= height-headerHeight;

        // Compute trailer position.
        Rect trailerRect= new Rect(x, yMax-buttonHeight, width, buttonHeight);
        remainingHeight-= buttonHeight+kMarginSize;
        
        // Compute variables & methods fix heights.
        float fixVariableHeight= 2f*titleHeight;
        float fixMethodHeight= titleHeight;
        remainingHeight-= fixVariableHeight+fixMethodHeight+2f*kMarginSize;
        
        // Compute varaibales & methods content heights.
        float variableContentHeight= labelHeight*NbOfVariables;
        int nbOfMethodsPerLine= (int)(width/MaxMethodWidth);
        if(nbOfMethodsPerLine < 1) nbOfMethodsPerLine= 1;
        float methodContentHeight  = labelHeight*((NbOfMethods+nbOfMethodsPerLine-1)/nbOfMethodsPerLine);
        float visibleVariableHeight= variableContentHeight;
        float visibleMethodHeight= methodContentHeight;
        if(visibleVariableHeight+visibleMethodHeight > remainingHeight) {
            float halfRemainingHeight= 0.5f*remainingHeight;
            if(visibleVariableHeight < halfRemainingHeight) {
                int nbOfMethodLines= (int)((remainingHeight-variableContentHeight)/labelHeight);
                visibleMethodHeight= labelHeight*nbOfMethodLines;
            } else if(visibleMethodHeight < halfRemainingHeight) {
                int nbOfVariableLines= (int)((remainingHeight-methodContentHeight)/labelHeight);
                visibleVariableHeight= labelHeight*nbOfVariableLines;
            } else {
                int nbOfVariableLines= (int)(halfRemainingHeight/labelHeight);
                visibleVariableHeight= labelHeight*nbOfVariableLines;
                int nbOfMethodLines= (int)((remainingHeight-visibleVariableHeight)/labelHeight);
                visibleMethodHeight= labelHeight*nbOfMethodLines;
            }
        }
        
        // Build variables & methods position.
        Rect boxVariableRect= new Rect(x, headerHeight, width, fixVariableHeight+visibleVariableHeight);
        Rect boxMethodRect  = new Rect(x, boxVariableRect.yMax+kMarginSize, width, fixMethodHeight+visibleMethodHeight);
        Rect scrollViewVariableRect= new Rect(x, boxVariableRect.y+fixVariableHeight, width, visibleVariableHeight);
        Rect scrollViewMethodRect  = new Rect(x, boxMethodRect.y+fixMethodHeight, width, visibleMethodHeight);
        Rect contentVariableRect= new Rect(0, 0, width-kScrollerSize, variableContentHeight);
        Rect contentMethodRect  = new Rect(0, 0, width-kScrollerSize, methodContentHeight);
        
        // Variables.
        GUI.Box(boxVariableRect,"");
        GUI.Label(new Rect(0.5f*(boxVariableRect.x+boxVariableRect.xMax-variableTitleSize.x), boxVariableRect.y, variableTitleSize.x, titleHeight), variableTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(scrollViewVariableRect, Vector2.zero, contentVariableRect, false, true);
        for(int i= 0; i < NbOfVariables; ++i) {
            ShowVariable(i, contentVariableRect.width, labelHeight);
        }
        GUI.EndScrollView();

        // Methods.
        GUI.Box(boxMethodRect, "");
        GUI.Label(new Rect(0.5f*(boxMethodRect.x+boxMethodRect.xMax-methodTitleSize.x), boxMethodRect.y, methodTitleSize.x, titleHeight), methodTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(scrollViewMethodRect, Vector2.zero, contentMethodRect, false, true);
        int column= 0;
        int row= 0;
        for(int i= 0; i < NbOfMethods; ++i) {            
            ShowMethod(i, column, row, MaxMethodWidth, labelHeight);
            if(++column >= nbOfMethodsPerLine) {
                column= 0;
                ++row;
            }
        }
        GUI.EndScrollView();

        // Display buttons.
        if(GUI.Button(new Rect(trailerRect.xMax-2f*buttonWidth-kMarginSize, trailerRect.y, buttonWidth, buttonHeight), cancelButton)) {
            Close();
        }
        if(GUI.Button(new Rect(trailerRect.xMax-buttonWidth, trailerRect.y, buttonWidth, buttonHeight), applyButton)) {
            Close();
        }
    }

    // ---------------------------------------------------------------------------------
    void ShowVariable(int id, float width, float height) {
        if(id >= Fields.Length+Properties.Length) return;
        string name;
        if(id < Fields.Length) {
            name= Fields[id].DisplayName;
        } else {
            name= Properties[id-Fields.Length].DisplayName;
        }
        GUI.Label(new Rect(0, id*height, width, height), name);
    }

    // ---------------------------------------------------------------------------------
    void ShowMethod(int id, int column, int row, float width, float height) {
        GUI.Button(new Rect(column*width, row*height, width, height), Methods[id].FunctionSignatureNoThis);                    
    }
}
