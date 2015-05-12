namespace iCanScript.Internal.Engine {
	// --------------------------------------------------------------------------
	/// Tag to indicate to use the script owner as port input.
	public class UndefinedTag {
		// ==========================================================================
		// FIELDS
		// --------------------------------------------------------------------------
		public static UndefinedTag instance= new UndefinedTag();
		
		// ==========================================================================
		// CONVERSIONS
		// --------------------------------------------------------------------------
		public override string ToString() {
			return "Undefined";
		}
	}
	
}
