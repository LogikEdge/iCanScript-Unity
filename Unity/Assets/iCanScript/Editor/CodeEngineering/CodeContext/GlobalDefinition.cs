using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class GlobalDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        string                myNamespace      = null;
        List<string>          myUsingDirectives= new List<string>();
        List<ClassDefinition> myClasses        = new List<ClassDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Namespace in which to generate the code.
        public string Namespace {
            get { return myNamespace; }
            set { myNamespace= value; }
        }

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the code global scope.
        public GlobalDefinition()
        : base(CodeType.GLOBAL) {
        }

        // -------------------------------------------------------------------
        /// Adds a class definition to the global scope
        ///
        /// @param classdefinition Class definition to add.
        ///
        public void AddClassDefinition(ClassDefinition classDefinition) {
            myClasses.Add(classDefinition);
            classDefinition.Parent= this;
        }
        
        // -------------------------------------------------------------------
        /// Adds a using directive to the file scope.
        ///
        /// @param usingDirective A string with a using directive.
        ///
        public void AddUsingDirective(string usingDirective) {
            if(!myUsingDirectives.Exists((s1)=> s1 == usingDirective)) {
                myUsingDirectives.Add(usingDirective);
            }
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate for code for the Visual Script.
        public override string GenerateCode(int indentSize) {
            // Generate using directives.
            var result= new StringBuilder(2048);
            result.Append(GenerateUsingDirectives());

            // Start generating the code from the namespace.
            if(string.IsNullOrEmpty(myNamespace)) {
                result.Append(GenerateClassDefinitions(0));
            }
            else {
                result.Append(GenerateNamespace(GenerateClassDefinitions));                
            }
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate for code for each class definition in the Visual Script.
        string GenerateClassDefinitions(int indentSize) {
            var result= new StringBuilder(1024);
            foreach(var classDef in myClasses) {
                result.Append("\n");
                result.Append(classDef.GenerateCode(indentSize));
            }
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for the using directives.
        ///
        /// @param usingDirectives Array of using target strings.
        /// @return Return the formatted code for the using diectives.
        ///
        string GenerateUsingDirectives() {
            StringBuilder result= new StringBuilder("");
            foreach(var u in myUsingDirectives) {
                result.Append("using ");
                result.Append(u);
                result.Append(";\n");
            }
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the code for the namespace
        ///
        /// @param namespaceName The namespace in which to generate the code.
        /// @return The formatted code for the namespace and its internals.
        ///
        string GenerateNamespace(CodeProducer namespaceBody) {
            StringBuilder result= new StringBuilder("\nnamespace ");
            result.Append(myNamespace);
            result.Append(" {\n");
            result.Append(namespaceBody(1));
            result.Append("\n}\n");
            return result.ToString();
        }
        
    }
}