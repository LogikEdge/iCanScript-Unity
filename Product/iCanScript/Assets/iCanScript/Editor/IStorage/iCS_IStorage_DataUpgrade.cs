#define TEST_UPGRADE
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;
using iCanScript.Nodes;

namespace iCanScript.Editor {
    
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
    		if(storageVersion.IsOlderThen(2,0,11)) {
                isUpgraded |= V2_0_11_EditorUpgrade();
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
        bool V2_0_11_EditorUpgrade() {
            bool isUpgraded= false;
            // -- Convert to new port specification --
            ForEach(
                p=> {
                    var engineObject= p.EngineObject;
                    var qualifiedTypeName= p.QualifiedTypeName;
                    if(qualifiedTypeName == "iCS_Package") {
                        engineObject.QualifiedType= typeof(iCS_Package).AssemblyQualifiedName;
                        isUpgraded= true;
                    }
                    else if(qualifiedTypeName == "iCS_State") {
                        engineObject.QualifiedType= typeof(iCS_State).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_StateChart") {
                        engineObject.QualifiedType= typeof(iCS_StateChart).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_Math") {
                        engineObject.QualifiedType= typeof(iCS_Math).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_FromTo") {
                        engineObject.QualifiedType= typeof(iCS_FromTo).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_VisualScriptImp") {
                        engineObject.QualifiedType= typeof(iCS_VisualScriptImp).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_ForceIntegrator") {
                        engineObject.QualifiedType= typeof(iCS_ForceIntegrator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_ImpulseForceGenerator") {
                        engineObject.QualifiedType= typeof(iCS_ImpulseForceGenerator).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_TimeUtility") {
                        engineObject.QualifiedType= typeof(iCS_TimeUtility).AssemblyQualifiedName;
                        isUpgraded= true;                        
                    }
                    else if(qualifiedTypeName == "iCS_DesiredVelocityForceGenerator") {
                        engineObject.QualifiedType= typeof(iCS_DesiredVelocityForceGenerator).AssemblyQualifiedName;
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
        /// Performs the editor data upgrade for v2.0.12.
        bool V2_0_12_EditorUpgrade() {
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
