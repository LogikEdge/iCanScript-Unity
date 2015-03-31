using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeGenerator {
        // -------------------------------------------------------------------
        public enum CodeType     { CLASS, FUNCTION, VARIABLE, PARAMETER };
        public enum AccessType   { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
        public enum ScopeType    { STATIC, NONSTATIC, VIRTUAL };
        public enum LocationType { LOCAL_TO_FUNCTION, LOCAL_TO_CLASS };
        
        // -------------------------------------------------------------------
        public delegate string CodeProducer(int indent);
    
        // -------------------------------------------------------------------
        CodeTree                   myCodeTree= null;
        
    	// -------------------------------------------------------------------------
        public void GenerateCodeFor(iCS_IStorage iStorage) {
            // Nothing to do if no or empty Visual Script.
            if(iStorage == null || iStorage.EditorObjects.Count == 0) {
                return;
            }
            // Build code context tree.
            myCodeTree= new CodeTree(iStorage);
            
            // Generate final code.
            var result= new StringBuilder(2048);
            result.Append(myCodeTree.GenerateCode());
            
            // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            // PREVIOUS CODE GENERATOR
    
            var className= iCS_TextUtility.ToCSharpName(iStorage.HostGameObject.name);
    
            // Write final code to file.
            CSharpFileUtils.WriteCSharpFile("iCanScript Generated Code", className, result.ToString());
        }
    
    }
    
}
