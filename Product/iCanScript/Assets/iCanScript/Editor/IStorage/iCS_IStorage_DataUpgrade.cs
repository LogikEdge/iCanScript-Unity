#define TEST_UPGRADE
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using iCanScript.Nodes;
using iCanScript.Variables;
using iCanScript.SimplePhysic;
using iCanScript.Conversions;
using iCanScript.TimeUtility;
using iCanScript.MathUtility;
using iCanScript.Logic;
using CSharp.Primitives;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ----------------------------------------------------------------------
        /// Performs engine data upgarde before generating the editor data.
    	void PerformEngineDataUpgrade() {
            bool isUpgraded= false;
        
            // PRE-PROCESSING ====================================================
            // Use this are to perform pre-processing data conversion.
    		iCS_Version softwareVersion= iCS_Version.Current;

    		// Extract the version of the storage.
    		iCS_Version storageVersion= new iCS_Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
    		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
            // -- Warn the user that an upgrade toke place --
            if(isUpgraded) {
    			ShowUpgradeDialog(softwareVersion);
            }
    	}
        // ----------------------------------------------------------------------
        /// Performs editor data upgarde.
    	void PerformEditorDataUpgrade() {
            bool isUpgraded= false;
        
            // PRE-PROCESSING ====================================================
            // Use this are to perform pre-processing data conversion.
    		iCS_Version softwareVersion= iCS_Version.Current;

    		// Extract the version of the storage.
    		iCS_Version storageVersion= new iCS_Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
    		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
            // -- Upgrade each version --
    		if(storageVersion.IsOlderThen(2,0,6)) {
                isUpgraded |= V2_0_6_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,9)) {
                isUpgraded |= V2_0_9_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,12)) {
                isUpgraded |= V2_0_12_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,17)) {
                isUpgraded |= V2_0_17_EditorUpgrade();
            }

            // -- Warn the user that an upgrade toke place --
            if(isUpgraded) {
                SaveStorage();
    			ShowUpgradeDialog(softwareVersion);
            }
    		// -- Update storage version identifiers --
#if TEST_UPGRADE
            Debug.LogWarning("ENABLE SAVED VERSION ONCE CONVERSION IS COMPLETED");
#else
    		EngineStorage.MajorVersion = iCS_Config.MajorVersion;
    		EngineStorage.MinorVersion = iCS_Config.MinorVersion;
    		EngineStorage.BugFixVersion= iCS_Config.BugFixVersion;
#endif
    	}

        // ----------------------------------------------------------------------
        /// Shows the upgrade information dialog box.
        ///
        /// @param softwareVersion The current version identifier.
        ///
    	void ShowUpgradeDialog(iCS_Version softwareVersion) {
    		EditorUtility.DisplayDialog("iCanScript Data Upgrade Required", "Your visual scripts were created with an earlier version of iCanScript.\n\nAn upgrade to v"+softwareVersion.ToString()+" will be performed in memory.\nPlease save your scenes to complete the upgrade.", "Ok");
    	}
        // ----------------------------------------------------------------------
    	void SaveCurrentScene() {
    		EditorApplication.SaveCurrentSceneIfUserWantsTo();
    	}

        // ======================================================================
        // UPGRADE CODE
        // ----------------------------------------------------------------------
        /// Performs the editor data upgrade for v2.0.6.
        bool V2_0_6_EditorUpgrade() {
            bool isUpgraded= false;
            // -- Scan for functions and properties nodes to add the self port --
            var needsSelfPort= new List<iCS_EditorObject>();
            ForEach(
                o=> {
                    if(o.IsKindOfFunction || o.IsInstanceNode) {
                        if(o.IsConstructor == false && GetSelfPort(o) == null) {
                            needsSelfPort.Add(o);
                        }
                    }
                }
            );
            foreach(var o in needsSelfPort) {
                CreateSelfPort(o);
                isUpgraded= true;            
            }
            return isUpgraded;
        }
        // ----------------------------------------------------------------------
        /// Performs the editor data upgrade for v2.0.9.
        bool V2_0_9_EditorUpgrade() {
            bool isUpgraded= false;
            if(string.IsNullOrEmpty(CSharpFileName)) {
                CSharpFileName= RootObject.CodeName;
                isUpgraded= true;
            }
            return isUpgraded;
        }
        // ----------------------------------------------------------------------
        /// Performs the editor data upgrade for v2.0.11.
        bool V2_0_12_EditorUpgrade() {
            bool isUpgraded= false;
            // -- Convert to new port specification --
            ForEach(
                p=> {
                    var engineObject= p.EngineObject;
                    var qualifiedTypeName= p.QualifiedTypeName;
                    if(qualifiedTypeName == "iCS_Package" || qualifiedTypeName == "iCanScript.Engine.iCS_Package") {
                        engineObject.QualifiedType= typeof(iCS_Package).AssemblyQualifiedName;
                        isUpgraded= true;
                    }
                    else if(qualifiedTypeName == "iCS_State" || qualifiedTypeName == "iCanScript.Engine.iCS_State") {
                        engineObject.QualifiedType= typeof(iCS_State).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_StateChart" || qualifiedTypeName == "iCanScript.Engine.iCS_StateChart") {
                        engineObject.QualifiedType= typeof(iCS_StateChart).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Math" || qualifiedTypeName == "iCanScript.Nodes.iCS_Math") {
                        engineObject.QualifiedType= typeof(Math).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_FromTo" || qualifiedTypeName == "iCanScript.Nodes.iCS_FromTo") {
                        engineObject.QualifiedType= typeof(FromTo).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_VisualScriptImp" || qualifiedTypeName == "iCanScript.Engine.iCS_VisualScriptImp") {
                        engineObject.QualifiedType= typeof(iCS_VisualScriptImp).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_ForceIntegrator" || qualifiedTypeName == "iCanScript.Nodes.iCS_ForceIntegrator") {
                        engineObject.QualifiedType= typeof(ForceIntegrator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_ImpulseForceGenerator" || qualifiedTypeName == "iCanScript.Nodes.iCS_ImpulseForceGenerator") {
                        engineObject.QualifiedType= typeof(ImpulseForceGenerator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_TimeUtility" || qualifiedTypeName == "iCanScript.Nodes.iCS_TimeUtility") {
                        engineObject.QualifiedType= typeof(DeltaTimeUtility).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_DesiredVelocityForceGenerator" || qualifiedTypeName == "iCanScript.Nodes.iCS_DesiredVelocityForceGenerator") {
                        engineObject.QualifiedType= typeof(DesiredVelocityForceGenerator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Variables" || qualifiedTypeName == "iCanScript.Nodes.iCS_Variables") {
                        engineObject.QualifiedType= typeof(SimpleVariables).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "Bool" || qualifiedTypeName == "iCanScript.Nodes.Bool") {
                        engineObject.QualifiedType= typeof(iCanScript.Variables.Bool).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "Int" || qualifiedTypeName == "iCanScript.Nodes.Int") {
                        engineObject.QualifiedType= typeof(iCanScript.Variables.Int).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "Float" || qualifiedTypeName == "iCanScript.Nodes.Float") {
                        engineObject.QualifiedType= typeof(iCanScript.Variables.Float).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_GameController" || qualifiedTypeName == "iCanScript.Nodes.iCS_GameController") {
                        engineObject.QualifiedType= typeof(GameControllerUtility).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Conditions" || qualifiedTypeName == "iCanScript.Nodes.iCS_Conditions") {
                        engineObject.QualifiedType= typeof(Conditions).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Boolean" || qualifiedTypeName == "iCanScript.Nodes.iCS_Boolean") {
                        engineObject.QualifiedType= typeof(Boolean).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Choice" || qualifiedTypeName == "iCanScript.Nodes.iCS_Choice") {
                        engineObject.QualifiedType= typeof(Choices).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Oscillator" || qualifiedTypeName == "iCanScript.Nodes.iCS_Oscillator") {
                        engineObject.QualifiedType= typeof(Oscillator).AssemblyQualifiedName;
                        isUpgraded= true;                      
                    }
                    else if(qualifiedTypeName == "iCS_PulseGenerator" || qualifiedTypeName == "iCanScript.Nodes.iCS_PulseGenerator") {
                        engineObject.QualifiedType= typeof(PulseGenerator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Timer" || qualifiedTypeName == "iCanScript.Nodes.iCS_Timer") {
                        engineObject.QualifiedType= typeof(Timer).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_TypeCasts" || qualifiedTypeName == "iCanScript.Nodes.iCS_TypeCasts") {
                        engineObject.QualifiedType= typeof(TypeCasts).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName.StartsWith("iCS")) {
                        Debug.LogWarning("Need to convert=> "+qualifiedTypeName);
                    }
                }
            );
            return isUpgraded;
        }
        // ----------------------------------------------------------------------
        /// Performs the editor data upgrade for v2.0.17.
        bool V2_0_17_EditorUpgrade() {
            bool isUpgraded= false;
            // -- Convert to new port specification --
            ForEach(
                p=> {
                    var engineObject= p.EngineObject;
                    var qualifiedTypeName= p.QualifiedTypeName;
                    if(qualifiedTypeName == "iCanScript.Logic.Boolean") {
                        var methodName= engineObject.MethodName;
                        var newQualifiedTypeName= typeof(CSharp.Primitives.Bool).AssemblyQualifiedName;
                        if(methodName == "Not" || methodName == "Inverse") {
                            engineObject.QualifiedType= newQualifiedTypeName;
                            engineObject.MethodName= "op_LogicalNot";
                            isUpgraded= true;
                        }
                        else if(methodName == "And") {
                            engineObject.QualifiedType= newQualifiedTypeName;
                            engineObject.MethodName= "op_BitwiseAnd";
                            isUpgraded= true;
                        }
                        else if(methodName == "Or") {
                            engineObject.QualifiedType= newQualifiedTypeName;
                            engineObject.MethodName= "op_BitwiseOr";
                            isUpgraded= true;
                        }
                        else if(methodName == "Xor") {
                            engineObject.QualifiedType= newQualifiedTypeName;
                            engineObject.MethodName= "op_ExclusiveOr";
                            isUpgraded= true;
                        }
                        else {
                            Debug.LogWarning("Need to convert=> "+qualifiedTypeName+"."+methodName);                            
                        }
                    }
                }
            );
            return isUpgraded;
        }
        // ----------------------------------------------------------------------
        /// Performs the editor data upgrade for v2.0.18.
        bool V2_0_18_EditorUpgrade() {
            bool isUpgraded= false;
            // -- Convert to new port specification --
            ForEach(
                p=> {
                    if(!p.IsPort) return;
                    // -- Abort if conversion already took place --
                    if(p.PortSpec != PortSpecification.Default) return;
                    // -- Initial setup of the port specification --
                    var parentNode= p.ParentNode;
                    if(p.IsEnablePort) {
                        p.PortSpec= PortSpecification.Enable;
                        isUpgraded= true;
                    }
                    else if(p.IsTriggerPort) {
                        p.PortSpec= PortSpecification.Trigger;
                        isUpgraded= true;
                    }
                    else if(p.IsTargetPort) {
                        p.PortSpec= PortSpecification.Target;
                        isUpgraded= true;
                    }
                    else if(p.IsSelfPort) {
                        p.PortSpec= PortSpecification.Self;
                        isUpgraded= true;
                    }
                    else if(p.IsReturnPort) {
                        p.PortSpec= PortSpecification.Return;
                        isUpgraded= true;
                    }
                    else if(parentNode.IsKindOfFunction) {
                        p.PortSpec= PortSpecification.Parameter;
                        isUpgraded= true;
                    }
                    else if(parentNode.IsEventHandler) {
                        if(p.IsFixDataPort) {
                            p.PortSpec= PortSpecification.Parameter;
                            isUpgraded= true;
                        }
                        else {
                            p.PortSpec= PortSpecification.PublicVariable;
                            isUpgraded= true;
                        }
                    }
                    else if(parentNode.IsPublicFunction) {
                        p.PortSpec= PortSpecification.Parameter;
                        isUpgraded= true;
                    }
                    else {
                    
                    }
                }
            );        
            return isUpgraded;
        }

    }

}
