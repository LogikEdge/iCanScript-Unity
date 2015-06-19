using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
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
		string myProjectIdent   = null;
		string myEngineNamespace= null;
		string myEditorNamespace= null;
		
		// ========================================================================
		// Properties
		// ------------------------------------------------------------------------
		public string ProjectName {
			get { return myProjectName; }
		}
		public string EngineNamespace {
			get { return myEngineNamespace; }
		}
		public string EditorNamespace {
			get { return myEditorNamespace; }
		}
		
		// ========================================================================
		// Creation/Destruction
		// ------------------------------------------------------------------------
		public ProjectInfo(string projectName) {
			myProjectName= projectName;
			myProjectIdent= NameUtility.ToTypeName(projectName);
			myEngineNamespace= myProjectIdent;
			myEditorNamespace= myEditorNamespace+".Editor";
		}

		// ========================================================================
		/// Parse project name.

		// ========================================================================
		/// Build folder structure for project.
		public void BuildFolders() {
			
		}
    }
}