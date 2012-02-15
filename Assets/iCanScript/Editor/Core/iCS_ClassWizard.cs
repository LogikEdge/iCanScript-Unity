using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : EditorWindow {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    class ControlPair {
        public iCS_ReflectionDesc  Component= null;
        public bool                IsActive= false;
        public ControlPair(iCS_ReflectionDesc component, bool isActive= false) {
            Component= component;
            IsActive= isActive;
        }
    };
    class VariablePair {
        public ControlPair InputControlPair= null;
        public ControlPair OutputControlPair= null;
        public VariablePair(iCS_ReflectionDesc inputComponent, bool inputActive, iCS_ReflectionDesc outputComponent, bool outputActive) {
            InputControlPair= new ControlPair(inputComponent, inputActive);
            OutputControlPair= new ControlPair(outputComponent, outputActive);
        }
    };
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_EditorObject        Target                = null;
    iCS_IStorage            Storage               = null;
    Type                    ClassType             = null;
    VariablePair[]          Fields                = null;
    VariablePair[]          Properties            = null;
    ControlPair[]           Methods               = null;
    Vector2                 VariableScrollPosition= Vector2.zero;
    Vector2                 MethodScrollPosition  = Vector2.zero;        
    
    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float       MaxMethodWidth   = 0;
    float       MaxVariableWidth = 0;
    float       VariableNameWidth= 0;
    
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
        Storage= storage;
        // Build class data.
        ClassType= target.RuntimeType;
        List<VariablePair> fields= new List<VariablePair>();
        List<VariablePair> properties= new List<VariablePair>();
        List<ControlPair> methods= new List<ControlPair>();
        iCS_ReflectionDesc[] components= iCS_DataBase.GetClassComponents(ClassType);
        foreach(var component in components) {
            bool isActive= Storage.ClassModuleFindFunction(Target, component) != null;
            if(component.IsField) {
                string name= component.FieldName;
                var variablePair= GetVariablePair(name, fields);
                if(component.IsSetField) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                        variablePair.InputControlPair.IsActive= isActive;
                    } else {
                        fields.Add(new VariablePair(component, isActive, null, false));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                        variablePair.OutputControlPair.IsActive= isActive;
                    } else {
                        fields.Add(new VariablePair(null, false, component, isActive));                                            
                    }
                }
                var fieldSize= EditorStyles.boldLabel.CalcSize(new GUIContent(name));
                if(fieldSize.x+kSpacer > MaxVariableWidth) {
                    MaxVariableWidth= fieldSize.x+kSpacer;
                }
            } else if(component.IsProperty) {
                string name= component.PropertyName;
                var variablePair= GetVariablePair(name, properties);
                if(component.IsSetProperty) {
                    if(variablePair != null) {
                        variablePair.InputControlPair.Component= component;
                        variablePair.InputControlPair.IsActive= isActive;
                    } else {
                        properties.Add(new VariablePair(component, isActive, null, false));                        
                    }
                } else {
                    if(variablePair != null) {
                        variablePair.OutputControlPair.Component= component;
                        variablePair.OutputControlPair.IsActive= isActive;
                    } else {
                        properties.Add(new VariablePair(null, false, component, isActive));                        
                    }
                }
                var propertySize= EditorStyles.boldLabel.CalcSize(new GUIContent(name));
                if(propertySize.x+kSpacer > MaxVariableWidth) {
                    MaxVariableWidth= propertySize.x+kSpacer;
                }
            } else {
                methods.Add(new ControlPair(component, isActive));
                var signatureSize= EditorStyles.boldLabel.CalcSize(new GUIContent(component.FunctionSignatureNoThis));
                if(signatureSize.x+12f > MaxMethodWidth) {
                    MaxMethodWidth= signatureSize.x+12f;
                }
            }
        }
        Fields= fields.ToArray();
    	Array.Sort(Fields, (x,y)=> GetAComponent(x).FieldName.CompareTo(GetAComponent(y).FieldName));
        Properties= properties.ToArray();
    	Array.Sort(Properties, (x,y)=> GetAComponent(x).PropertyName.CompareTo(GetAComponent(y).PropertyName));
        Methods= methods.ToArray();
    	Array.Sort(Methods, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));
        Target= target;
        Repaint();
    }
    
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        Target= null;
        Storage= null;
    }
    // ---------------------------------------------------------------------------------
    void OnDisable() {
        Target= null;
        Storage= null;
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(Target == null) return;
        
        EditorGUIUtility.LookLikeInspector();
        
        // Define GUI content.
        GUIContent constructorLabel= new GUIContent("Create Instance");
        GUIContent variableTitle   = new GUIContent("Variables");
        GUIContent inTitle         = new GUIContent("In");
        GUIContent outTitle        = new GUIContent("Out");
        GUIContent nameTitle       = new GUIContent("Name");
        GUIContent typeTitle       = new GUIContent("Type");
        GUIContent methodTitle     = new GUIContent("Methods");
        
        // Compute content size.
        Vector2 labelSize= EditorStyles.label.CalcSize(constructorLabel);
        Vector2 variableTitleSize= EditorStyles.boldLabel.CalcSize(variableTitle);
        Vector2 inTitleSize= EditorStyles.boldLabel.CalcSize(inTitle);
        Vector2 outTitleSize= EditorStyles.boldLabel.CalcSize(outTitle);
        Vector2 nameTitleSize= EditorStyles.boldLabel.CalcSize(nameTitle);
        Vector2 typeTitleSize= EditorStyles.boldLabel.CalcSize(typeTitle);
        Vector2 methodTitleSize= EditorStyles.boldLabel.CalcSize(methodTitle);
        float labelHeight= 4f+labelSize.y;
        float titleHeight= 4f+variableTitleSize.y;

        // Compute window parameters.
        float x     = kMarginSize;
        float width = position.width-2f*kMarginSize;
        float height= position.height-2f*kMarginSize;
        
        // Compute header position.
        float headerHeight= 3f*labelSize.y+kMarginSize;
        float remainingHeight= height-headerHeight;

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
                visibleMethodHeight= labelHeight*(nbOfMethodLines+1);
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
        GUI.Label(new Rect(0.5f*(boxVariableRect.x+boxVariableRect.xMax-variableTitleSize.x), boxVariableRect.y, variableTitleSize.x, titleHeight), variableTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+kSpacer+0.5f*(kCheckBoxWidth-inTitleSize.x),   boxVariableRect.y+titleHeight, inTitleSize.x,   titleHeight), inTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+kSpacer+kCheckBoxWidth+0.5f*(kCheckBoxWidth-outTitleSize.x),  boxVariableRect.y+titleHeight, outTitleSize.x,  titleHeight), outTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth, boxVariableRect.y+titleHeight, nameTitleSize.x, titleHeight), nameTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth+VariableNameWidth, boxVariableRect.y+titleHeight, typeTitleSize.x, titleHeight), typeTitle, EditorStyles.boldLabel);
        GUI.Box(new Rect(scrollViewVariableRect.x, scrollViewVariableRect.y-3f, scrollViewVariableRect.width, 3),"");
        VariableScrollPosition= GUI.BeginScrollView(scrollViewVariableRect, VariableScrollPosition, contentVariableRect, false, true);
        for(int i= 0; i < NbOfVariables; ++i) {
            ShowVariable(i, contentVariableRect.width, labelHeight);
        }
        GUI.EndScrollView();

        // Methods.
        GUI.Box(boxMethodRect, "");
        GUI.Label(new Rect(0.5f*(boxMethodRect.x+boxMethodRect.xMax-methodTitleSize.x), boxMethodRect.y, methodTitleSize.x, titleHeight), methodTitle, EditorStyles.boldLabel);
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
            bool prevActive= inputControlPair.IsActive;
            inputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-checkBoxSize.x), y, kCheckBoxWidth, height), inputControlPair.IsActive, "");
            if(prevActive != inputControlPair.IsActive) {
                if(inputControlPair.IsActive) {
                    Storage.ClassModuleCreate(Target, inputControlPair.Component);
                } else {
                    Storage.ClassModuleDestroy(Target, inputControlPair.Component);
                }                
            }
        }
        x+= kCheckBoxWidth;
        ControlPair outputControlPair= variablePair.OutputControlPair;
        if(outputControlPair.Component != null) {
            bool prevActive= outputControlPair.IsActive;
            outputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-checkBoxSize.x), y, kCheckBoxWidth, height), outputControlPair.IsActive, "");
            if(prevActive != outputControlPair.IsActive) {
                if(outputControlPair.IsActive) {
                    Storage.ClassModuleCreate(Target, outputControlPair.Component);
                } else {
                    Storage.ClassModuleDestroy(Target, outputControlPair.Component);
                }                
            }
        }
        x+= kCheckBoxWidth+kSpacer;
        GUIStyle labelStyle= inputControlPair.IsActive || outputControlPair.IsActive ? EditorStyles.boldLabel : EditorStyles.label;
        GUI.Label(new Rect(x, y, VariableNameWidth, height), name, labelStyle);
        x+= VariableNameWidth;
        GUI.Label(new Rect(x, y, width-x, height), typeName, labelStyle);
    }

    // ---------------------------------------------------------------------------------
    void ShowMethod(int id, int column, int row, float width, float height) {
        GUIStyle style= GUI.skin.button;
        var alignment= style.alignment;
        var fontStyle= style.fontStyle;
        var textColor= style.normal.textColor;        
        var background= style.normal.background;
        style.alignment= TextAnchor.MiddleLeft;
        if(Methods[id].IsActive) {
            style.normal.textColor= Color.white;
            style.fontStyle= FontStyle.Bold;
            style.normal.background= style.active.background;            
        } else {
            style.fontStyle= FontStyle.Italic;
        }
        if(GUI.Button(new Rect(column*width, row*height, width, height), Methods[id].Component.FunctionSignatureNoThis)) {
            Methods[id].IsActive^= true;
            if(Methods[id].IsActive) {
                Storage.ClassModuleCreate(Target, Methods[id].Component);
            } else {
                Storage.ClassModuleDestroy(Target, Methods[id].Component);
            }
        }
        style.normal.textColor= textColor;
        style.normal.background= background;
        style.fontStyle= fontStyle;
        style.alignment= alignment;
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
