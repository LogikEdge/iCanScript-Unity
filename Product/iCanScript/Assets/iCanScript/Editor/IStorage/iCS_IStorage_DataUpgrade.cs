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
    		if(storageVersion.IsOlderThen(2,0,17)) {
                isUpgraded |= V2_0_17_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,20)) {
                isUpgraded |= V2_0_20_EditorUpgrade();
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
        /// Upgrade port specification in preparation for user configurable
        /// port specification.
        bool V2_0_20_EditorUpgrade() {
            // -- Erase any stored port specification, --
            ForEach(
                p=> {
                    if(!p.IsPort) return;
                    p.PortSpec= PortSpecification.Default;
                }
            );
            // -- Convert to new port specification --
            bool isUpgraded= false;
            bool newPassNeeded= true;
            while(newPassNeeded) {
                newPassNeeded= false;
                ForEach(
                    p=> {
                        if(!p.IsPort) return;
                        // -- Abort if conversion already took place --
                        if(p.PortSpec != PortSpecification.Default) return;
                        // -- Setup spec for control ports. --
                        var parentNode= p.ParentNode;
                        var producerPort= p.SegmentProducerPort;
                        if(p.IsEnablePort) {
                            p.PortSpec= PortSpecification.Enable;
                            isUpgraded= true;
                        }
                        else if(p.IsTriggerPort) {
                            p.PortSpec= PortSpecification.Trigger;
                            isUpgraded= true;
                        }
                        // -- Connected port follow the producer. --
                        else if(producerPort != p) {
                            if(producerPort.PortSpec != PortSpecification.Default) {
                                p.PortSpec= producerPort.PortSpec;
                                isUpgraded= true;                                        
                            }
                            else {
                                newPassNeeded= true;
                            }                                    
                        }
                        else if(parentNode.IsFunctionDefinition) {
                            if(p.IsInDataPort) {
                                if(producerPort == p) {
                                    p.PortSpec= PortSpecification.Parameter;
                                }
                                else {
                                    if(producerPort.PortSpec != PortSpecification.Default) {
                                        p.PortSpec= producerPort.PortSpec;                                        
                                    }
                                    else {
                                        newPassNeeded= true;
                                    }
                                }
                            }
                            if(p.IsOutDataPort) {
                                if(p.ConsumerPorts.Length == 0) {
                                    p.PortSpec= PortSpecification.Parameter;
                                }
                                else {
                                    if(producerPort.PortSpec != PortSpecification.Default) {
                                        p.PortSpec= producerPort.PortSpec;                                        
                                    }
                                    else {
                                        newPassNeeded= true;
                                    }                                    
                                }
                            }
                            isUpgraded= true;
                        }
                        else if(parentNode.IsEventHandler) {
                            if(p.IsFixDataPort) {
                                p.PortSpec= PortSpecification.Parameter;
                            }
                            else {
                                p.PortSpec= PortSpecification.PublicVariable;
                            }
                            isUpgraded= true;
                        }
                        else if(parentNode.IsVariableDefinition) {
                            if(p.IsOutDataPort) {
                                p.PortSpec= PortSpecification.PublicVariable;
                            }
                            else if(p.IsInDataPort && producerPort == p) {
                                p.PortSpec= PortSpecification.Constant;
                            }
                            else {
                                p.PortSpec= PortSpecification.LocalVariable;
                            }
                            isUpgraded= true;
                        }
                        // TODO: Needs to be verified...
                        else if(parentNode.IsKindOfFunction) {
                            if(p.IsInDataPort) {
                                if(producerPort == p) {
                                    var initialValue= p.InitialValue;
                                    if(initialValue != null) {
                                        p.PortSpec= PortSpecification.Constant;                                        
                                    }
                                    else {
                                        p.PortSpec= PortSpecification.PublicVariable;
                                    }
                                }
                                else {
                                    p.PortSpec= PortSpecification.LocalVariable;
                                }
                            }
                            else if(p.IsOutDataPort) {
                                if(GraphInfo.MustBeATypeVariable(p)) {
                                    p.PortSpec= PortSpecification.PrivateVariable;
                                }
                                else {
                                    p.PortSpec= PortSpecification.LocalVariable;
                                }
                            }
                            isUpgraded= true;
                        }
                        else if(parentNode.IsKindOfPackage) {
                            if(p.IsInDataPort) {
                                if(producerPort == p) {
                                    var initialValue= p.InitialValue;
                                    if(initialValue != null) {
                                        p.PortSpec= PortSpecification.Constant;
                                        isUpgraded= true;
                                    }
                                    else {
                                        p.PortSpec= PortSpecification.PublicVariable;
                                        isUpgraded= true;
                                    }
                                }
                            }
                        }
                        else {
                            p.PortSpec= PortSpecification.LocalVariable;
                            isUpgraded= true;
                        }
                    }
                );        
            }
            return isUpgraded;
        }

    }

}
