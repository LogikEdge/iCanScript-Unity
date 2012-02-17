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
    ControlPair[]           Constructors          = null;
    Vector2                 VariableScrollPosition= Vector2.zero;
    Vector2                 MethodScrollPosition  = Vector2.zero;        
    
    // =================================================================================
    // Layout info.
    // ---------------------------------------------------------------------------------
    float       MaxConstructorWidth= 0f;
    float       MaxMethodWidth     = 0f;
    float       MaxVariableWidth   = 0f;
    float       VariableNameWidth  = 0f;

    // =================================================================================
    // Constant GUI Content
    // ---------------------------------------------------------------------------------
    bool        IsGUIConstantInit= false;
    GUIContent  InstanceTitle= new GUIContent("Instance");
    GUIContent  VariableTitle= new GUIContent("Variables");
    GUIContent  InTitle      = new GUIContent("In");
    GUIContent  OutTitle     = new GUIContent("Out");
    GUIContent  NameTitle    = new GUIContent("Name");
    GUIContent  TypeTitle    = new GUIContent("Type");
    GUIContent  MethodTitle  = new GUIContent("Operations");
    Vector2     LabelSize;
    Vector2     InstanceTitleSize;
    Vector2     VariableTitleSize;
    Vector2     InTitleSize;
    Vector2     OutTitleSize;
    Vector2     NameTitleSize;
    Vector2     TypeTitleSize;
    Vector2     MethodTitleSize;
    Vector2     CheckBoxSize;    
    float       LabelHeight;
    float       TitleHeight;
    float       HeaderHeight;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float   kSpacer       = 8f;
    const float   kMarginSize   = 10f;
    const float   kScrollerSize = 16f;
    const float   kCheckBoxWidth= 25f;
    
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    int NbOfVariables { get { return Fields.Length + Properties.Length; }}
    int NbOfMethods   { get { return Methods.Length; }}
    
    // ---------------------------------------------------------------------------------
    void Init() {
        Target= null;
        Storage= null;
        IsGUIConstantInit= false;
    }
    // ---------------------------------------------------------------------------------
    public void Activate(iCS_EditorObject target, iCS_IStorage storage) {
        if(target == null || storage == null) {
            Init();
            return;
        }
        Storage= storage;
        // Build class data.
        ClassType= target.RuntimeType;
        List<VariablePair> fields= new List<VariablePair>();
        List<VariablePair> properties= new List<VariablePair>();
        List<ControlPair> constructors= new List<ControlPair>();
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
            } else if(component.IsConstructor) {
                isActive= false;
                iCS_EditorObject existing= Storage.ClassModuleGetConstructor(Target);
                if(existing != null && component.Method == existing.GetMethodBase(Storage.EditorObjects)) {
                    isActive= true;
                }
                constructors.Add(new ControlPair(component, isActive));
                var constructorSize= EditorStyles.boldLabel.CalcSize(new GUIContent(component.FunctionSignatureNoThisNoOutput));
                if(constructorSize.x+12f > MaxConstructorWidth) {
                    MaxConstructorWidth= constructorSize.x+12f;
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
        Constructors= constructors.ToArray();
    	Array.Sort(Constructors, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));        
        Methods= methods.ToArray();
    	Array.Sort(Methods, (x,y)=> x.Component.FunctionSignatureNoThis.CompareTo(y.Component.FunctionSignatureNoThis));
        Target= target;
        Repaint();
    }
    // ---------------------------------------------------------------------------------
    public void Deactivate() {
        Init();
    }
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        Init();

    }
    // ---------------------------------------------------------------------------------
    void OnDisable() {
        Init();
    }
    // ---------------------------------------------------------------------------------
    void InitConstantGUIContent() {
        if(IsGUIConstantInit) return;
        IsGUIConstantInit= true;
        // Compute content size.
        LabelSize        = EditorStyles.label.CalcSize(new GUIContent("abc")); 
        InstanceTitleSize= EditorStyles.boldLabel.CalcSize(InstanceTitle);
        VariableTitleSize= EditorStyles.boldLabel.CalcSize(VariableTitle);
        InTitleSize      = EditorStyles.boldLabel.CalcSize(InTitle);
        OutTitleSize     = EditorStyles.boldLabel.CalcSize(OutTitle);
        NameTitleSize    = EditorStyles.boldLabel.CalcSize(NameTitle);
        TypeTitleSize    = EditorStyles.boldLabel.CalcSize(TypeTitle);
        MethodTitleSize  = EditorStyles.boldLabel.CalcSize(MethodTitle);
                         
        LabelHeight      = 4f+LabelSize.y;
        TitleHeight      = 4f+InTitleSize.y;
        HeaderHeight     = 3f*LabelSize.y+kMarginSize;
                         
        CheckBoxSize     = GUI.skin.toggle.CalcSize(new GUIContent(""));                
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(Target == null) return;
        InitConstantGUIContent();
        EditorGUIUtility.LookLikeInspector();
        
        // Compute window parameters.
        float x     = kMarginSize;
        float width = position.width-2f*kMarginSize;
        float height= position.height-2f*kMarginSize;
        
        // Compute header position.
        Rect headerRect= new Rect(x, 0, width, HeaderHeight);
        float remainingHeight= height-HeaderHeight;

        // Compute variables & methods fix heights.
        float fixVariableHeight= 2f*TitleHeight;
        float fixMethodHeight= TitleHeight;
        remainingHeight-= fixVariableHeight+fixMethodHeight+2f*kMarginSize;
        
        // Compute varaibales & methods content heights.
        float variableContentHeight= LabelHeight*NbOfVariables;
        int nbOfMethodsPerLine= (int)(width/MaxMethodWidth);
        if(nbOfMethodsPerLine < 1) nbOfMethodsPerLine= 1;
        float methodContentHeight  = LabelHeight*((NbOfMethods+nbOfMethodsPerLine-1)/nbOfMethodsPerLine);
        float visibleVariableHeight= variableContentHeight;
        float visibleMethodHeight= methodContentHeight;
        if(visibleVariableHeight+visibleMethodHeight > remainingHeight) {
            float halfRemainingHeight= 0.5f*remainingHeight;
            if(visibleVariableHeight < halfRemainingHeight) {
                int nbOfMethodLines= (int)((remainingHeight-variableContentHeight)/LabelHeight);
                visibleMethodHeight= LabelHeight*nbOfMethodLines;
            } else if(visibleMethodHeight < halfRemainingHeight) {
                int nbOfVariableLines= (int)((remainingHeight-methodContentHeight)/LabelHeight);
                visibleVariableHeight= LabelHeight*nbOfVariableLines;
            } else {
                int nbOfVariableLines= (int)(halfRemainingHeight/LabelHeight);
                visibleVariableHeight= LabelHeight*nbOfVariableLines;
                int nbOfMethodLines= (int)((remainingHeight-visibleVariableHeight)/LabelHeight);
                visibleMethodHeight= LabelHeight*(nbOfMethodLines+1);
            }
        }
        
        // Build variables & methods position.
        Rect boxVariableRect= new Rect(x, HeaderHeight, width, fixVariableHeight+visibleVariableHeight);
        Rect boxMethodRect  = new Rect(x, boxVariableRect.yMax+kMarginSize, width, fixMethodHeight+visibleMethodHeight);
        Rect scrollViewVariableRect= new Rect(x, boxVariableRect.y+fixVariableHeight, width, visibleVariableHeight);
        Rect scrollViewMethodRect  = new Rect(x, boxMethodRect.y+fixMethodHeight, width, visibleMethodHeight);
        Rect contentVariableRect= new Rect(0, 0, width-kScrollerSize, variableContentHeight);
        Rect contentMethodRect  = new Rect(0, 0, width-kScrollerSize, methodContentHeight);
        ComputeVariableContentLayout(contentMethodRect.width);
        
        // Display Header.
        CenterTitle(headerRect, Target.Name);
        ShowConstructor(headerRect);
        
        // Display Variables.
        GUI.Box(boxVariableRect,"");
        CenterTitle(boxVariableRect, VariableTitle, VariableTitleSize);
        GUI.Label(new Rect(x+kSpacer+0.5f*(kCheckBoxWidth-InTitleSize.x),   boxVariableRect.y+TitleHeight, InTitleSize.x,   TitleHeight), InTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+kSpacer+kCheckBoxWidth+0.5f*(kCheckBoxWidth-OutTitleSize.x),  boxVariableRect.y+TitleHeight, OutTitleSize.x,  TitleHeight), OutTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth, boxVariableRect.y+TitleHeight, NameTitleSize.x, TitleHeight), NameTitle, EditorStyles.boldLabel);
        GUI.Label(new Rect(x+2f*kSpacer+2f*kCheckBoxWidth+VariableNameWidth, boxVariableRect.y+TitleHeight, TypeTitleSize.x, TitleHeight), TypeTitle, EditorStyles.boldLabel);
        GUI.Box(new Rect(scrollViewVariableRect.x, scrollViewVariableRect.y-3f, scrollViewVariableRect.width, 3),"");
        VariableScrollPosition= GUI.BeginScrollView(scrollViewVariableRect, VariableScrollPosition, contentVariableRect, false, true);
        for(int i= 0; i < NbOfVariables; ++i) {
            ShowVariable(i, contentVariableRect.width, LabelHeight);
        }
        GUI.EndScrollView();

        // Display Methods.
        GUI.Box(boxMethodRect, "");
        CenterTitle(boxMethodRect, MethodTitle, MethodTitleSize);
        MethodScrollPosition= GUI.BeginScrollView(scrollViewMethodRect, MethodScrollPosition, contentMethodRect, false, true);
        int column= 0;
        int row= 0;
        for(int i= 0; i < NbOfMethods; ++i) {            
            ShowMethod(i, column, row, MaxMethodWidth, LabelHeight);
            if(++column >= nbOfMethodsPerLine) {
                column= 0;
                ++row;
            }
        }
        GUI.EndScrollView();
    }

    // ---------------------------------------------------------------------------------
    void ShowConstructor(Rect headerRect) {
        float y= headerRect.y+TitleHeight;
        GUI.Label(new Rect(headerRect.x, y, InstanceTitleSize.x, InstanceTitleSize.y), InstanceTitle, EditorStyles.boldLabel);
        float x= 2f*kSpacer+headerRect.x+InstanceTitleSize.x;
        // Fill-in available constructors.
        string[] instanceOptions= new string[1+Constructors.Length];
        instanceOptions[0]= "Use input port (this)";
        int instanceIdx= -1;
        for(int i= 0; i < Constructors.Length; ++i) {
            instanceOptions[i+1]= Constructors[i].Component.FunctionSignatureNoThisNoOutput;
            if(Constructors[i].IsActive) instanceIdx= i;
        }
        ++instanceIdx;
        float maxWidth= headerRect.width-x;
        float width= Mathf.Min(maxWidth, MaxConstructorWidth);
        int newIdx= EditorGUI.Popup(new Rect(x, y, width, LabelHeight), instanceIdx, instanceOptions, EditorStyles.toolbarPopup);
        if(newIdx != instanceIdx) {
            if(instanceIdx != 0) {
                Constructors[instanceIdx-1].IsActive= false;
                Storage.ClassModuleDestroyConstructor(Target);
            }
            if(newIdx != 0) {
                Constructors[newIdx-1].IsActive= true;
                Storage.ClassModuleCreateConstructor(Target, Constructors[newIdx-1].Component);
            }
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
        ControlPair inputControlPair= variablePair.InputControlPair;
        if(inputControlPair.Component != null) {
            bool prevActive= inputControlPair.IsActive;
            inputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-CheckBoxSize.x), y, kCheckBoxWidth, height), inputControlPair.IsActive, "");
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
            outputControlPair.IsActive= GUI.Toggle(new Rect(x+0.5f*(kCheckBoxWidth-CheckBoxSize.x), y, kCheckBoxWidth, height), outputControlPair.IsActive, "");
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
    void CenterTitle(Rect rect, string title) {
        var content= new GUIContent(title);
        var contentSize= EditorStyles.boldLabel.CalcSize(content);
        CenterTitle(rect, content, contentSize);
    }
    void CenterTitle(Rect rect, GUIContent content, Vector2 contentSize) {
        CenterLabel(rect, content, contentSize, EditorStyles.boldLabel);
    }
    void CenterLabel(Rect rect, GUIContent content, Vector2 contentSize, GUIStyle style) {
        GUI.Label(new Rect(rect.x+0.5f*(rect.width-contentSize.x), rect.y, contentSize.x, contentSize.y), content, style);        
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
