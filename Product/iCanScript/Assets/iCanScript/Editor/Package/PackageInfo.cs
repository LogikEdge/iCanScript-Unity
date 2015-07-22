using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using iCanScript.Internal.JSON;

namespace iCanScript.Internal.Editor {
	using CodeParsing;
	
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class contains all the project related information.
    ///
    /// The content is saved inside the project file.
    ///
    public class PackageInfo {
		// ========================================================================
		// Fields
		// ------------------------------------------------------------------------
        public  string      myVersion       = null;
		public  string      myPackageName   = "";
        public  bool        myIsNested      = false;
        private string      myRelativeFolder= "";
        private PackageInfo myParentPackage = null;
		
		// ========================================================================
		// Properties
		// ------------------------------------------------------------------------
		public string PackageName {
			get { return myPackageName ?? ""; }
			set { UpdatePackageName(value); }
		}
		public string PackageVersion {
			get { return myVersion; }
			set { myVersion= value; }
		}
        public PackageInfo ParentPackage {
            get {
                return myParentPackage;
            }
            set {
                myParentPackage= value;
                myIsNested= value != null;
            }
        }
        public bool IsNested { get { return myIsNested; }}
        public string ParentFolder {
            get {
                if(myParentPackage == null || IsRootPackage) return "";
                return myParentPackage.GetRelativePackageFolder();
            }
        }
		public string EngineNamespace {
			get { return GetEngineNamespace(); }
		}
		public string EditorNamespace {
			get { return GetEditorNamespace(); }
		}
		// ========================================================================
        /// Determines if the project already exists.
        public bool AlreadyExists {
            get {
                var filePath= GetAbsoluteFileNamePath();
                return File.Exists(filePath);                
            }
        }
		public bool IsRootPackage {
			get {
                var rootPackageName= UnityUtility.GetProjectName();
				return PackageName == rootPackageName && ParentPackage == null;
			}
		}
		// ========================================================================
		// Creation/Destruction
		// ------------------------------------------------------------------------
		public PackageInfo() {
            DefaultConstructor();
		}
		// ------------------------------------------------------------------------
        void DefaultConstructor() {
            var packageName= UnityUtility.GetProjectName();
            ParentPackage= null;
    		UpdatePackageName(packageName);
        }
		// ------------------------------------------------------------------------
		public PackageInfo(string packageName, PackageInfo parent) {
			if(packageName == null) {
                DefaultConstructor();
                return;
            }
            ParentPackage= parent;
			UpdatePackageName(packageName);
		}

		// ========================================================================
		/// Computes the package file name.
        ///
        /// @return The iCanScript file name including its extension.
        ///
		public string GetFileName() {
			return myPackageName+".icspackage";
		}
		
		// ========================================================================
		/// Creates and return the relative package folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The project folder path relative to the Assets folder.
        ///
        public string GetRelativePackageFolder(bool doCreate= false) {
            // -- Special case for the root package. --
            if(IsRootPackage) return "";
            var packagePath= myRelativeFolder;
            if(string.IsNullOrEmpty(packagePath)) {
                if(myParentPackage == null) {
                    packagePath= myPackageName;
                }
                else {
                    var parentPath= myParentPackage.GetRelativePackageFolder();
                    var separator= string.IsNullOrEmpty(parentPath) ? "" : "/";
                    packagePath= parentPath+separator+myPackageName;                    
                }
            }
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(packagePath);                
            }
            return packagePath;
		}
		
		// ========================================================================
		/// Sets the package folder path.
        ///
        /// @param path The package path.
        ///
        public void SetPackageFolder(string path) {
            if(IsRootPackage) {
                myRelativeFolder= "";
                return;
            }
            if(string.IsNullOrEmpty(path)) {
                myRelativeFolder= myPackageName;
                return;
            }
            if(path.StartsWith(Application.dataPath)) {
                var dpLen= Application.dataPath.Length;
                path= path.Substring(dpLen, path.Length-dpLen);
                if(path.Length > 0 && path[0] == '/') {
                    path= path.Substring(1, path.Length-1);
                }
            }
            myRelativeFolder= path;
		}
		
		// ========================================================================
		/// Creates and returns the absolute package folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript project folder.
        ///
		public string GetPackageFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativePackageFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative engine visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The engine visual script folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEngineVisualScriptFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativePackageFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var visualScriptPath= projectPath+separator+"Visual Scripts";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(visualScriptPath);                
            }
            return visualScriptPath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute engine visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript engine visual script
        ///         folder.
        ///
		public string GetEngineVisualScriptFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEngineVisualScriptFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative engine generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The engine generated code folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEngineGeneratedCodeFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativePackageFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var generatedCodePath= projectPath+separator+"Generated Code";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(generatedCodePath);                
            }
            return generatedCodePath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute engine generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript engine generated code
        ///         folder.
        ///
		public string GetEngineGeneratedCodeFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEngineGeneratedCodeFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative editor visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The editor visual script folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEditorVisualScriptFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativePackageFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var visualScriptPath= projectPath+separator+"Editor/Visual Scripts";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(visualScriptPath);                
            }
            return visualScriptPath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute editor visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript editor visual script
        ///         folder.
        ///
		public string GetEditorVisualScriptFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEditorVisualScriptFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative editor generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The editor generated code folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEditorGeneratedCodeFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativePackageFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var generatedCodePath= projectPath+separator+"Editor/Generated Code";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(generatedCodePath);                
            }
            return generatedCodePath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute editor generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript editor generated code
        ///         folder.
        ///
		public string GetEditorGeneratedCodeFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEditorGeneratedCodeFolder(doCreate);
		}
		
		// ========================================================================
		/// Computes the relative path of the project file.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        ///
		public string GetRelativeFileNamePath(bool doCreate= false) {
            var relativePath= GetRelativePackageFolder(doCreate);
            if(!string.IsNullOrEmpty(relativePath)) {
                relativePath+= "/";
            }
			return relativePath + GetFileName();
		}
		
		// ========================================================================
        /// Computes the absolute path of the project file.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        ///
        public string GetAbsoluteFileNamePath(bool doCreate= false) {
			var projectPath= GetRelativePackageFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var fileName= GetFileName();
            return Folder.AssetToAbsolutePath(projectPath+separator+fileName);            
        }
        
		// ========================================================================
		/// Extracts the engine namespace from the project name.
		public string GetEngineNamespace() {
            // -- Translate '-' to '_' for the namespace. --
            var formattedProjectName= NameUtility.ToTypeName(myPackageName.Replace('-','_'));
            if(myParentPackage == null) return formattedProjectName;
            return myParentPackage.EngineNamespace+"."+formattedProjectName;
		}
		
		// ========================================================================
		/// Extracts the editor namespace from the project name.
		public string GetEditorNamespace() {
			return GetEngineNamespace()+".Editor";
		}
		
		// ========================================================================
		/// Parse project name.
		///
		/// @param projectName The full name of the project.
		/// @return An array of the project name constituents.
		///
		static string[] SplitPackageName(string projectName) {
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
		void UpdatePackageName(string projectName) {
			myPackageName= projectName;
            // -- Force first character ident rules for project name. --
            while(myPackageName.Length > 0 && !iCS_TextUtil.IsIdent1(myPackageName[0])) {
                myPackageName= myPackageName.Substring(1);
            }
            // -- Accept ident rule or '-' for remaining of the project name. --
            for(int i= 1; i < myPackageName.Length; ++i) {
                var c= myPackageName[i];
                if(!iCS_TextUtil.IsIdent(c) && c != '-' && c != ' ') {
                    UpdatePackageName(myPackageName.Substring(0,i)+myPackageName.Substring(i+1));
                    return;
                }
            }
		}
		
		// ========================================================================
		/// Save and Update the package information.
		public void Save() {
            // -- Create the project folders (if not existing). --
			var projectPath= GetRelativePackageFolder(true);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            // -- Update version information. --
            myVersion= Version.Current.ToString();
            // -- Save the project information. --
            var fileName= GetFileName();
            var filePath= Folder.AssetToAbsolutePath(projectPath+separator+fileName);
            JSONFile.PrettyWrite(filePath, this);
		}
		
		// ========================================================================
		/// Removes all files associated with the iCanScript package.
        public void RemovePackage() {
            // -- Create the project folders (if not existing). --
			var projectPath= GetRelativePackageFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
			AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Visual Scripts");
            AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Generated Code");
           	AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Editor");
            var fileName= GetFileName();
            AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+fileName);
            if(Folder.IsEmpty(GetPackageFolder())) {
				if(!string.IsNullOrEmpty(projectPath)) {
	                AssetDatabase.DeleteAsset("Assets/"+projectPath);
				}
            }
        }
        
		// ========================================================================
		/// Removes all files associated with the iCanScript package.
        ///
        /// @param absolutePath The absolute path of the package file.
        ///
        public static void RemovePackage(string absolutePath) {
			var project= PackageInfo.Load(absolutePath);
			project.RemovePackage();
        }
        
		// ========================================================================
		/// Creates a new Project info from the given JSON root object.
        ///
        /// @param jsonRoot The JSON root object from which to extract the package
        ///                 information.
        ///
        public static PackageInfo Load(JObject jsonRoot) {
            // -- Read the package information from the JSON string. --
            JString version    = jsonRoot.GetValueFor("myVersion") as JString;
            JString packageName= jsonRoot.GetValueFor("myPackageName") as JString;
            JBool   isNested   = jsonRoot.GetValueFor("myIsNested") as JBool;
            // -- Don't create a package if core infromation is not present. --
            if(version == null || version.isNull ||
               packageName == null || packageName.isNull ||
               isNested == null || isNested.isNull) {
                return null;
            }
            // -- Set the package with the information red from the JSON file. --
            var newProject= new PackageInfo();
            newProject.myVersion    = version.value;
            newProject.myPackageName= packageName.value;
            newProject.myIsNested   = isNested.value;

//            newProject.myParentFolder       = parentFolder.value;
            return newProject;
        }

        // =================================================================================
        /// Load the package file from disk.
        ///
        /// @param absolutePath The absolute path of the project file.
        /// @info The active project set to the newly loaded project.
        /// 
        public static PackageInfo Load(string absolutePath, bool declareError= false) {
            var jsonRoot= JSONFile.Read(absolutePath);
            if(jsonRoot == null || jsonRoot.isNull) {
                if(declareError) {
                    Debug.LogError("iCanScript: Unable to load package at=> "+absolutePath);                    
                }
                return null;
            }
            return Load(jsonRoot);
        }
    }
}