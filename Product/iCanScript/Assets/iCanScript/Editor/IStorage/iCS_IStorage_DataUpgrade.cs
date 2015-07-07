//#define TEST_UPGRADE
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using iCanScript.Nodes;
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
    		Version softwareVersion= Version.Current;

    		// Extract the version of the storage.
    		Version storageVersion= new Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
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
    		Version softwareVersion= Version.Current;

    		// Extract the version of the storage.
    		Version storageVersion= new Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
    		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
            // -- Upgrade each version --
    		if(storageVersion.IsOlderThen(2,0,17)) {
                isUpgraded |= V2_0_17_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,20)) {
                isUpgraded |= V2_0_20_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,21)) {
                isUpgraded |= V2_0_21_EditorUpgrade();
            }
    		if(storageVersion.IsOlderThen(2,0,24)) {
                isUpgraded |= V2_0_24_EditorUpgrade();
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
    	void ShowUpgradeDialog(Version softwareVersion) {
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
            ForEach(
                p=> {
                    if(!p.IsPort) return;
                    // -- Abort if conversion already took place --
                    if(p.PortSpec != PortSpecification.Default) return;
                    // -- Default the port spec. --
                    GraphEditor.SetDefaultPortSpec(p);
                    if(p.PortSpec != PortSpecification.Default) {
                        isUpgraded= true;
                    }
                }
            );
            return isUpgraded;
        }

        // ======================================================================
        /// Fix invalid parameter indexes for function definition.
        bool V2_0_21_EditorUpgrade() {
            // -- Test and fix parameter indexes --
            bool isUpgraded= false;
            ForEach(
                n=> {
                    if(!n.IsFunctionDefinition) return;
                    GraphEditor.AdjustPortIndexes(n);
                    isUpgraded= true;
                }
            );
            return isUpgraded;
        }
        // ======================================================================
        /// Set default base type for engine scripts.
        bool V2_0_24_EditorUpgrade() {
            // -- Test and fix parameter indexes --
            bool isUpgraded= false;
			if(!IsEditorScript && !BaseTypeOverride) {
				BaseType= "UnityEngine.MonoBehaviour";
				isUpgraded= true;
			}
            return isUpgraded;
        }
    }
}
