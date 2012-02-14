using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassModuleMenu : EditorWindow {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    class ControlPair : Prelude.Tuple<iCS_ReflectionDesc, bool> {
        public ControlPair(iCS_ReflectionDesc component, bool isActive= false) {
            Component= component;
            IsActive= isActive;
        }
        public iCS_ReflectionDesc Component { get { return Item1; } set { Item1= value; }}
        public bool               IsActive  { get { return Item2; } set { Item2= value; }}
    };
    class VariablePair : Prelude.Tuple<ControlPair, ControlPair> {
        public VariablePair(iCS_ReflectionDesc inputComponent, iCS_ReflectionDesc outputComponent) {
            InputControlPair= new ControlPair(inputComponent);
            OutputControlPair= new ControlPair(outputComponent);
        }
        public ControlPair InputControlPair  { get { return Item1; } set { Item1= value; }}
        public ControlPair OutputControlPair { get { return Item2; } set { Item2= value; }}
    };
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject        Target                = null;
    Type                    ClassType             = null;
    bool                    IsCreateInstance      = false;
    int                     ConstructorIdx        = 0;
    string[]                Constructors          = new String[1]{"None"};
    VariablePair[]          Fields                = null;
    VariablePair[]          Properties            = null;
    ControlPair[]           Methods               = null;
    Vector2                 VariableScrollPosition= Vector2.zero;
    Vector2                 MethodScrollPosition  = Vector2.zero;        
    
    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float   MaxMethodWidth   = 0;
    float   MaxVariableWidth = 0;
    float   VariableNameWidth= 0;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float   kSpacer       = 8;
    const float   kMarginSize   = 10;
    const float   kScrollerSize = 16;
    const float   kCheckBoxWidth= 25;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    int NbOfVariables { get { return Fields.Length + Properties.Length; }}
    int NbOfMethods   { get { return Methods.Length; }}
    
    // ---------------------------------------------------------------------------------
    public void Activate(iCS_EditorObject target, iCS_IStorage storage) {
        ClassType= target.RuntimeType;
        List<VariablePair> fields= new List<VariablePair>();
        List<VariablePair> properties= new List<VariablePair>();
        List<ControlPair> methods= new List<ControlPair>();
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(ClassType);
        foreach(var component in components) {
            if(component.IsField) {
                string name= component.FieldName;
                var variablePair= GetVariablePair(name, fields);
                if(component.IsSetField) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                    } else {
                        fields.Add(new VariablePair(component, null));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                    } else {
                        fields.Add(new VariablePair(null, component));                                            
                    }
                }
                var fieldSize= EditorStyles.label.CalcSize(new GUIContent(name));
                if(fieldSize.x+kSpacer > MaxVariableWidth) {
                    MaxVariableWidth= fieldSize.x+kSpacer;
                }
            } else if(component.IsProperty) {
                string name= component.PropertyName;
                var variablePair= GetVariablePair(name, properties);
                if(component.IsSetProperty) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                    } else {
                        properties.Add(new VariablePair(component, null));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                    } else {
                        properties.Add(new VariablePair(null, component));                        
                    }
                }
                var propertySize= EditorStyles.label.CalcSize(new GUIContent(name));
                if(propertySize.x+kSpacer > MaxVariableWidth) {
                    MaxVariableWidth= propertySize.x+kSpacer;
                }
            } else {
                methods.Add(new ControlPair(component));
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
        GUIContent inTitle         = new GUIContent("In");
        GUIContent outTitle        = new GUIContent("Out");
        GUIContent nameTitle       = new GUIContent("Name");
        GUIContent typeTitle       = new GUIContent("Type");
        GUIContent methodTitle     = new GUIContent("Methods");
        
        // Compute content size.
        Vector2 labelSize= EditorStyles.label.CalcSize(constructorLabel);
        Vector2 variableTitleSize= EditorStyles.whiteLargeLabel.CalcSize(variableTitle);
        Vector2 inTitleSize= EditorStyles.whiteLargeLabel.CalcSize(inTitle);
        Vector2 outTitleSize= EditorStyles.whiteLargeLabel.CalcSize(outTitle);
        Vector2 nameTitleSize= EditorStyles.whiteLargeLabel.CalcSize(nameTitle);
        Vector2 typeTitleSize= EditorStyles.whiteLargeLabel.CalcSize(typeTitle);
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
        ComputeVariableContentLayout(contentMethodRect.width);
        
        // Variables.
        GUI.Box(boxVariableRect,"");
        GUI.Label(new Rect(0.5f*(boxVariableRect.x+boxVariableRect.xMax-variableTitleSize.x), boxVariableRect.y, variableTitleSize.x, titleHeight), variableTitle, EditorStyles.whiteLargeLabel);
        GUI.Label(new Rect(x+kSpacer+0.5f*(kCheckBoxWidth-inTitleSize.x),   boxVariableRect.y+titleHeight, inTitleSize.x,   titleHeight), inTitle, EditorStyles.whiteLargeLabel);
        GUI.Label(new Rect(x+kSpacer+kCheckBoxWidth+0.5f*(kCheckBoxWidth-outTitleSize.x),  boxVariableRect.y+titleHeight, outTitleSize.x,  titleHeight), outTitle, EditorStyles.whiteLargeLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth, boxVariableRect.y+titleHeight, nameTitleSize.x, titleHeight), nameTitle, EditorStyles.whiteLargeLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth+VariableNameWidth, boxVariableRect.y+titleHeight, typeTitleSize.x, titleHeight), typeTitle, EditorStyles.whiteLargeLabel);
        GUI.Box(new Rect(scrollViewVariableRect.x, scrollViewVariableRect.y-3f, scrollViewVariableRect.width, 3),"");
        VariableScrollPosition= GUI.BeginScrollView(scrollViewVariableRect, VariableScrollPosition, contentVariableRect, false, true);
        for(int i= 0; i < NbOfVariables; ++i) {
            ShowVariable(i, contentVariableRect.width, labelHeight);
        }
        GUI.EndScrollView();

        // Methods.
        GUI.Box(boxMethodRect, "");
        GUI.Label(new Rect(0.5f*(boxMethodRect.x+boxMethodRect.xMax-methodTitleSize.x), boxMethodRect.y, methodTitleSize.x, titleHeight), methodTitle, EditorStyles.whiteLargeLabel);
        MethodScrollPosition= GUI.BeginScrollView(scrollViewMethodRect, MethodScrollPosition, contentMethodRect, false, true);
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
        width-= 2f*kSpacer;
        if(id >= Fields.Length+Properties.Length) return;
        string name;
        string typeName;
        VariablePair variablePair;
        if(id < Fields.Length) {
            variablePair= Fields[id];
            var field= GetAComponent(variablePair);
            name= field.FieldName;
            typeName= iCS_Types.TypeName(field.FieldType);
        } else {
            variablePair= Properties[id-Fields.Length];
            var property= GetAComponent(variablePair);
            name= property.PropertyName;
            typeName= iCS_Types.TypeName(property.PropertyType);
        }
        float x= kSpacer;
        float y= id*height;
        var checkBoxSize= GUI.skin.toggle.CalcSize(new GUIContent(""));
        ControlPair inputControlPair= variablePair.InputControlPair;
        if(inputControlPair.Component != null) {
            inputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-checkBoxSize.x), y, kCheckBoxWidth, height), inputControlPair.IsActive, "");
        }
        x+= kCheckBoxWidth;
        ControlPair outputControlPair= variablePair.OutputControlPair;
        if(outputControlPair.Component != null) {
            outputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-checkBoxSize.x), y, kCheckBoxWidth, height), outputControlPair.IsActive, "");
        }
        x+= kCheckBoxWidth+kSpacer;
        GUI.Label(new Rect(x, y, VariableNameWidth, height), name);
        x+= VariableNameWidth;
        GUI.Label(new Rect(x, y, width-x, height), typeName);
    }

    // ---------------------------------------------------------------------------------
    void ShowMethod(int id, int column, int row, float width, float height) {
        GUI.Button(new Rect(column*width, row*height, width, height), Methods[id].Component.FunctionSignatureNoThis);                    
    }
    

    // =================================================================================
    // Layout
    // ---------------------------------------------------------------------------------
    void ComputeVariableContentLayout(float width) {
        width-= 2f*kSpacer;
        float labelWidth= 0.5f*(width-2f*kCheckBoxWidth);
        VariableNameWidth= labelWidth < MaxVariableWidth ? MaxVariableWidth : labelWidth;        
    }
    
    // =================================================================================
    // Helpers
    // ---------------------------------------------------------------------------------
    iCS_ReflectionDesc GetAComponent(VariablePair pair) {
        return pair.InputControlPair.Component ?? pair.OutputControlPair.Component; 
    }
    VariablePair GetVariablePair(string name, List<VariablePair> lst) {
        foreach(var pair in lst) {
            iCS_ReflectionDesc inputComponent= pair.InputControlPair.Component;
            if(inputComponent != null) {
                if(inputComponent.IsField) {
                    if(inputComponent.FieldName == name) return pair;
                } else {
                    if(inputComponent.PropertyName == name) return pair;
                }
            }
            iCS_ReflectionDesc outputComponent= pair.OutputControlPair.Component;
            if(outputComponent != null) {
                if(outputComponent.IsField) {
                    if(outputComponent.FieldName == name) return pair;
                } else {
                    if(outputComponent.PropertyName == name) return pair;
                }                
            }
        }
        return null;
    }
}
