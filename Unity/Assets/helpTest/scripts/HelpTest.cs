using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;



public class HelpTest : MonoBehaviour {


	string getHelp(string searchAPI, string section) {

		// Create Filename that will contain help in Unity folder.
		string path = "/Applications/Unity/Unity.app/Contents/Documentation/html/en/ScriptReference";
		string fileName = string.Concat (searchAPI, ".html");

		// Find file if it exists
		string[] helpFiles = Directory.GetFiles (path, fileName);

		// Variables use "-" instead of "." in filename, try replacing "." with "-" if we did not find it the first time.
		if (helpFiles.Length == 0) {
			fileName = string.Concat ( searchAPI.Replace('.', '-'), ".html");
			helpFiles = Directory.GetFiles (path, fileName);
		}

		if (helpFiles.Length != 0) {
				string helpFile = helpFiles [0];

				// Create an XML document from the file (even though it is HTML)
				XmlDocument doc = new XmlDocument ();
				doc.Load (helpFile);
				XmlNodeReader reader = new XmlNodeReader (doc);
		
				string currentText;
				string helpText="";
				
				// Parse through the "XML"
				while (reader.Read()) {
					currentText= reader.ReadString();

					// Look for "Section"
					if(currentText == section ) {
						while(reader.Read ()) {
							// Description will be broken up in formating, keep concatinating the bits and pieces.
							currentText = reader.ReadString();
							helpText= string.Concat (helpText, currentText);

							// If there is another header, or a format change, description is probably over.
							if(reader.Name=="h2" || reader.Name=="pre")
								return helpText;
						}
					}

				}
		}
		return "";
					
	} 


	public string inputString = "";
	private string previousInputString = "";
	private string descriptionText= "";
	private string parameterText= "";
	void OnGUI() {
	
		GUI.Label( new Rect(30,30,Screen.width,Screen.height/2), string.Concat("Description:\n\n",descriptionText));
		GUI.Label( new Rect(30,(Screen.height/2)+60 ,Screen.width,Screen.height/2), string.Concat("Parameters:\n\n",parameterText));

		inputString = GUI.TextField(new Rect(10, 10, Screen.width-20, 20), inputString);


		if (previousInputString != inputString) {
			descriptionText= getHelp(inputString, "Description");
			parameterText= getHelp(inputString, "Parameters");
			previousInputString = inputString;					
		}
	}
}
