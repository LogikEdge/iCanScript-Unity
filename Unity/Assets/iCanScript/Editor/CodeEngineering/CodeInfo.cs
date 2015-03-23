using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeInfo {
        public enum CodeType     { CLASS, FUNCTION, VARIABLE, PARAMETER };
        public enum AccessType   { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
        public enum ScopeType    { STATIC, NONSTATIC, VIRTUAL };
        public enum LocationType { LOCAL_TO_FUNCTION, LOCAL_TO_CLASS };
        
        public string       name        = null;
        public CodeType     codeType    = CodeType.CLASS;
        public AccessType   accessType  = AccessType.PUBLIC;
        public ScopeType    scopeType   = ScopeType.NONSTATIC;
        public LocationType locationType= LocationType.LOCAL_TO_CLASS;
    }

}