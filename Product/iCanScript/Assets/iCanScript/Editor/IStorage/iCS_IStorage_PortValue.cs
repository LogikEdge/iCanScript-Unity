using UnityEngine;
using System;
using System.Collections;
using iCanScript;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // Port Archiving / Initial Value
        // ----------------------------------------------------------------------
    	public void LoadInitialPortValueFromArchive(iCS_EditorObject port) {
    		if(!port.IsInDataOrControlPort) return;
    		if(port.ProducerPortId != -1) return;
            port.InitialValue= GetInitialPortValueFromArchive(port);
    	}
        // ----------------------------------------------------------------------
    	public object GetInitialPortValueFromArchive(iCS_EditorObject port) {
    		if(!port.IsInDataOrControlPort) return null;
    		if(port.ProducerPortId != -1) return null;
    		if(iCS_Strings.IsEmpty(port.InitialValueArchive)) {
    			return null;
    		}
    		iCS_Coder coder= new iCS_Coder(port.InitialValueArchive);
            try {
        		return coder.DecodeObjectForKey("InitialValue", Storage);
            }
            catch  {
                return null;
            }
    	}
        // ----------------------------------------------------------------------
        public void StoreInitialPortValueInArchive(iCS_EditorObject port) {
            if(port.InitialValue == null) {
                port.InitialValueArchive= null;
                return;
            }
    		iCS_Coder coder= new iCS_Coder();
    		coder.EncodeObject("InitialValue", port.InitialValue, Storage);
    		port.InitialValueArchive= coder.Archive;         
        }
    }

}
