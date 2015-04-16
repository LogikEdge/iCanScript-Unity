using UnityEngine;
using System.Collections;

namespace iCanScript.Engine {
	// --------------------------------------------------------------------------
	/// Tag to indicate to use the script owner as port input.
	public class OwnerTag {
		// ==========================================================================
		// FIELDS
		// --------------------------------------------------------------------------
		public static OwnerTag instance= new OwnerTag();
		
		// ==========================================================================
		// CONVERSIONS
		// --------------------------------------------------------------------------
		public override string ToString() {
			return "Owner";
		}
	}
	
}
