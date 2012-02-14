using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class iCS_ClassModuleMenu : EditorWindow {
    // ---------------------------------------------------------------------------------
    Type        ClassType       = null;
    bool        IsCreateInstance= false;
    int         ConstructorIdx  = 0;
    string[]    Constructors    = new String[1]{"None"};
    int         NbOfVariables   = 5;
    int         NbOfMethods     = 15;
    int         MaxMethodWidth  = 200;
    
    // ---------------------------------------------------------------------------------
    const int   MarginSize= 10;
    
    // ---------------------------------------------------------------------------------
    void OnGUI() {
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
        float labelHeight= labelSize.y;
        float titleHeight= variableTitleSize.y;
        float buttonWidth= Mathf.Max(applyButtonSize.x, cancelButtonSize.x);
        float buttonHeight= 4f+Mathf.Max(applyButtonSize.y, cancelButtonSize.y);

        // Compute window parameters.
        float x     = MarginSize;
        float y     = MarginSize;
        float width = position.width-2f*MarginSize;
        float height= position.height-2f*MarginSize;
        float xMax  = x+width;
        float yMax  = y+height;
        
        // Compute header position.
        float headerHeight= 3f*labelSize.y+MarginSize;
        Rect headerRect= new Rect(x, y, width, headerHeight);
        float remainingHeight= height-headerHeight;

        // Compute trailer position.
        float trailerHeight= applyButtonSize.y;
        Rect trailerRect= new Rect(x, yMax-buttonHeight, width, buttonHeight);
        remainingHeight-= buttonHeight+MarginSize;
        
        // Compute variables & methods fix heights.
        float fixVariableHeight= 2f*titleHeight;
        float fixMethodHeight= titleHeight;
        remainingHeight-= fixVariableHeight+fixMethodHeight+2f*MarginSize;
        
        // Compute varaibales & methods content heights.
        float variableContentHeight= labelHeight*NbOfVariables;
        int nbOfMethodPerLines= (int)(width/MaxMethodWidth);
        if(nbOfMethodPerLines < 1) nbOfMethodPerLines= 1;
        float methodContentHeight  = labelHeight*((NbOfMethods+nbOfMethodPerLines-1)/nbOfMethodPerLines);
        if(variableContentHeight+methodContentHeight > remainingHeight) {
            float halfRemainingHeight= 0.5f*remainingHeight;
            if(variableContentHeight < halfRemainingHeight) {
                int nbOfMethodLines= (int)((remainingHeight-variableContentHeight)/labelHeight);
                methodContentHeight= labelHeight*nbOfMethodLines;
            } else if(methodContentHeight < halfRemainingHeight) {
                int nbOfVariableLines= (int)((remainingHeight-methodContentHeight)/labelHeight);
                variableContentHeight= labelHeight*nbOfVariableLines;
            } else {
                int nbOfVariableLines= (int)(halfRemainingHeight/labelHeight);
                variableContentHeight= labelHeight*nbOfVariableLines;
                int nbOfMethodLines= (int)((remainingHeight-variableContentHeight)/labelHeight);
                methodContentHeight= labelHeight*nbOfMethodLines;
            }
        }
        
        // Build variables & methods position.
        Rect boxVariableRect= new Rect(x, headerHeight, width, fixVariableHeight+variableContentHeight);
        Rect boxMethodRect  = new Rect(x, boxVariableRect.yMax+MarginSize, width, fixMethodHeight+methodContentHeight);
        Rect contentVariableRect= new Rect(x, boxVariableRect.y+fixVariableHeight, width, variableContentHeight);
        Rect contentMethodRect  = new Rect(x, boxMethodRect.y+fixMethodHeight, width, methodContentHeight);
        
        // Variables.
        GUI.Box(boxVariableRect,"");
        GUI.Label(new Rect(0.5f*(boxVariableRect.x+boxVariableRect.xMax-variableTitleSize.x), boxVariableRect.y, variableTitleSize.x, titleHeight), variableTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(contentVariableRect, Vector2.zero, new Rect(0,0,width, variableContentHeight));
        for(int i= 0; i < NbOfVariables; ++i) {
            GUI.Label(new Rect(0,i*labelHeight,100,labelHeight), "V"+i);            
        }
        GUI.EndScrollView();

        // Methods.
        GUI.Box(boxMethodRect, "");
        GUI.Label(new Rect(0.5f*(boxMethodRect.x+boxMethodRect.xMax-methodTitleSize.x), boxMethodRect.y, methodTitleSize.x, titleHeight), methodTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(contentMethodRect, Vector2.zero, new Rect(0,0,width, methodContentHeight));
        for(int i= 0; i < NbOfMethods; ++i) {
            GUI.Label(new Rect(0,i*labelHeight,100,labelHeight), "M"+i);            
        }
        GUI.EndScrollView();

        // Display buttons.
        if(GUI.Button(new Rect(trailerRect.xMax-2f*buttonWidth-MarginSize, trailerRect.y, buttonWidth, buttonHeight), cancelButton)) {
            Close();
        }
        if(GUI.Button(new Rect(trailerRect.xMax-buttonWidth, trailerRect.y, buttonWidth, buttonHeight), applyButton)) {
            Close();
        }
    }
}
