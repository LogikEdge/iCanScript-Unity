using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;



public class HelpTest : MonoBehaviour {


	string getHelpDescription(string findHelpOn) {

		// Create Filename that will contain help in Unity folder.
		string path = "/Applications/Unity/Unity.app/Contents/Documentation/html/en/ScriptReference";
		string fileName = string.Concat (findHelpOn, ".html");

		// Find file if it exists
		string[] helpFiles = Directory.GetFiles (path, fileName);

		if (helpFiles.Length != 0) {
				string helpFile = helpFiles [0];

				// Create an XML document from the file (even though it is HTML)
				XmlDocument doc = new XmlDocument ();
				doc.Load (helpFile);
				XmlNodeReader reader = new XmlNodeReader (doc);
		
				
				string descriptionText= "";	
				string currentText;

				
				// Parse through the "XML"
				while (reader.Read()) {
					currentText= reader.ReadString();
					// Look for "Description"
					if(currentText == "Description") {
						while(reader.Read () ) {
							// Description will be broken up in formating, keep concatinating the bits and pieces.
							currentText = reader.ReadString();
							descriptionText= string.Concat (descriptionText, currentText);
							// If there is another header, or a format change, description is probably over.
							if(reader.Name=="h2" || reader.Name=="pre")
						 	  break;
						}
						return descriptionText;
					}
				}
		}
		return "";
					
	} 

		


	public string inputString = "";
	private string previousInputString = "";
	private string helpDescription= "";
	void OnGUI() {
	
		GUI.Label( new Rect(30,30,Screen.width,Screen.height), helpDescription);

		inputString = GUI.TextField(new Rect(10, 10, 200, 20), inputString,40);


		if (previousInputString != inputString) {
				helpDescription = getHelpDescription (inputString);
				previousInputString = inputString;					
		}
	}
}
