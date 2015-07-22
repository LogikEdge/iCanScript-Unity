using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class InlineCodeDefinition : CodeBase {

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a statement block local variable.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created type.
        ///
        public InlineCodeDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {
        }

        // -------------------------------------------------------------------
        /// Returns a list of all enable ports that affects this function call.
        public override iCS_EditorObject[] GetRelatedEnablePorts() {
            return ControlFlow.GetAllRelatedEnablePorts(VSObject);
        }

        // -------------------------------------------------------------------
        /// Returns the list of all visual script objects this function call
        /// depends on.
        public override iCS_EditorObject[] GetDependencies() {
            return GetNodeCodeDependencies(VSObject);
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the function call.
        ///
//        public override string GenerateHeader(int identSize) {
//            return ToIndent(identSize);
//        }

        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            var code= VSObject.Value;
            if(code == null) return "";
            var ident= ToIndent(indentSize);
            return ReformatUserCode(code as string, ident);
        }

        // ===================================================================
        /// Generate the global scope trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the global scope.
        ///
//        public override string GenerateTrailer(int identSize) {
//            return "\n";
//        }


        // ===================================================================
        /// Reformats the user supplied code to make it similar to the
        /// visual script generated code.
        ///
        /// @param code The user supplied code string.
        /// @return The reformatted code string.
        ///
        string ReformatUserCode(string code, string ident) {
            // -- Split the user code into lines. --
            var lines= TextUtility.SplitIntoLines(code);
            // -- Remove trailing blank lines and white characters. --
            TextUtility.TrimEnd(ref lines);
            var nbOfLines= lines.Length;
            if(nbOfLines == 0) return "";
            // -- Assure that the code ends properly. --
            var lastLine= lines[nbOfLines-1];
            if(lastLine.Length > 0 && lastLine[0] != '#') {
                var lastChar= lastLine[lastLine.Length-1];
                if(lastChar != ';' && lastChar != '}') {
                    lastLine+= ";";
                }                
            }
            lines[nbOfLines-1]= lastLine;
            // -- Remove starting white spaces. --
            int maxWhiteSpacesToRemove= 1000;
            for(int i= 0; i < nbOfLines; ++i) {
                lines[i]= TextUtility.TrimStartingWhiteSpaces(lines[i], ref maxWhiteSpacesToRemove);
            }
            
            // -- Fold back the lines into code. --
            var result= new StringBuilder(128);
            for(int i= 0; i < nbOfLines; ++i) {
                var line= lines[i];
                if(line.Length > 0 && line[0] != '#') {
                    result.Append(ident);                    
                }
                result.Append(line);
                result.Append("\n");
            }
            return result.ToString();
        }
    }

}