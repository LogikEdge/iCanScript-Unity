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
    int         NbOfVariables   = 50;
    int         NbOfMethods     = 150;
    int         MaxMethodWidth  = 200;
    
    // ---------------------------------------------------------------------------------
    const int   MarginSize  = 10;
    const int   ScrollerSize= 16;
    
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
        Rect boxMethodRect  = new Rect(x, boxVariableRect.yMax+MarginSize, width, fixMethodHeight+visibleMethodHeight);
        Rect scrollViewVariableRect= new Rect(x, boxVariableRect.y+fixVariableHeight, width, visibleVariableHeight);
        Rect scrollViewMethodRect  = new Rect(x, boxMethodRect.y+fixMethodHeight, width, visibleMethodHeight);
        Rect contentVariableRect= new Rect(0, 0, width-ScrollerSize, variableContentHeight);
        Rect contentMethodRect  = new Rect(0, 0, width-ScrollerSize, methodContentHeight);
        
        // Variables.
        GUI.Box(boxVariableRect,"");
        GUI.Label(new Rect(0.5f*(boxVariableRect.x+boxVariableRect.xMax-variableTitleSize.x), boxVariableRect.y, variableTitleSize.x, titleHeight), variableTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(scrollViewVariableRect, Vector2.zero, contentVariableRect, false, true);
        for(int i= 0; i < NbOfVariables; ++i) {
            GUI.Label(new Rect(0,i*labelHeight,100,labelHeight), "V"+i);
        }
        GUI.EndScrollView();

        // Methods.
        GUI.Box(boxMethodRect, "");
        GUI.Label(new Rect(0.5f*(boxMethodRect.x+boxMethodRect.xMax-methodTitleSize.x), boxMethodRect.y, methodTitleSize.x, titleHeight), methodTitle, EditorStyles.whiteLargeLabel);
        GUI.BeginScrollView(scrollViewMethodRect, Vector2.zero, contentMethodRect, false, true);
        int column= 0;
        int row= 0;
        for(int i= 0; i < NbOfMethods; ++i) {            
            GUI.Button(new Rect(column*MaxMethodWidth,row*labelHeight,MaxMethodWidth,labelHeight), "M"+i);            
            if(++column >= nbOfMethodsPerLine) {
                column= 0;
                ++row;
            }
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
