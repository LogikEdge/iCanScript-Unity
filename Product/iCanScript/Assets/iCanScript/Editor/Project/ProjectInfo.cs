using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor {
	using CodeParsing;
	
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class contains all the project related information.
    ///
    /// The content is saved inside the project file.
    ///
    public class ProjectInfo {
		// ========================================================================
		// Fields
		// ------------------------------------------------------------------------
		string myProjectName    = null;
		string myNamespace      = null;
		string myEditorNamespace= null;
		
		// ========================================================================
		// Properties
		// ------------------------------------------------------------------------
		public string ProjectName {
			get { return myProjectName; }
			set { UpdateProjectName(value); }
		}
		public string Namespace {
			get { return myNamespace; }
			set { UpdateNamespace(value); }
		}
		public string EditorNamespace {
			get { return myEditorNamespace; }
			set { myEditorNamespace= value; }
		}
		
		// ========================================================================
		// Creation/Destruction
		// ------------------------------------------------------------------------
		public ProjectInfo(string projectName) {
			UpdateProjectName(projectName);
		}

		// ========================================================================
		/// Parse project name.
		static string[] SplitProjectName(string projectName) {
			// -- Convert file path to namespace format. --
			projectName= projectName.Replace('/', '.'); 
			// -- Remove all illegal characters. --
			var cleanName= new StringBuilder(64);
			var len= projectName.Length;
			for(int i= 0; i < len; ++i) {
				char c= projectName[i];
				if(cleanName.Length == 0) {
					if(ParsingUtility.IsFirstIdent(c)) {
						cleanName.Append(c);
					}
				}
				else if(Char.IsWhiteSpace(c) || c == '.' || ParsingUtility.IsIdent(c)) {
					cleanName.Append(c);
				}
				else {
					cleanName.Append(' ');
				}
			}
			// -- Split the name into its parts. --
			var splitName= cleanName.ToString().Split(new Char[]{'.'});
			// -- Convert each part to a type format. --
			for(int i= 0; i < splitName.Length; ++i) {
				splitName[i]= NameUtility.ToTypeName(splitName[i]);
			}
			return splitName;
		}
		
		// ========================================================================
		/// Update project name.
		///
		/// @param projectName The new project name.
		///
		void UpdateProjectName(string projectName) {
			if(myProjectName == projectName) return;
			myProjectName= projectName;
			var splitName= SplitProjectName(projectName);
			UpdateNamespace(iCS_TextUtility.CombineWith(splitName, "."));
		}
		
		// ========================================================================
		/// Update base namespace.
		///
		/// @param projectName The new project name.
		///
		void UpdateNamespace(string newNamespace) {
			myNamespace= newNamespace;
			myEditorNamespace= myNamespace+".Editor";			
		}
		
		// ========================================================================
		/// Resets the namespaces to their default values.
		public void ResetNamespaces() {
			var splitName= SplitProjectName(myProjectName);
			UpdateNamespace(iCS_TextUtility.CombineWith(splitName, "."));			
		}
		
		// ========================================================================
		/// Save and Update the project information.
		public void Save() {
			Debug.Log("Saving the project: "+myProjectName);
		}
		
		// ========================================================================
		/// Build folder structure for project.
		public void BuildFolders() {
			
		}
    }
}