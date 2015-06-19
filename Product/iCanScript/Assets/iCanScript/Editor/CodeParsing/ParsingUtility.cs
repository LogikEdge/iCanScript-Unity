using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeParsing {

	public static class ParsingUtility {

		// ========================================================================
		/// Determines if the given _Char_ is a valid first character for an ident.
		///
		/// @param c The character to verify.
		/// @return _true_ if valid first ident character. _false_ otherwise.
		///
		public static bool IsFirstIdent(Char c) {
			return Char.IsLetter(c) || c == '_';
		}

		// ========================================================================
		/// Determines if the given _Char_ is a valid ident character.
		///
		/// @param c The character to verify.
		/// @return _true_ if valid ident character. _false_ otherwise.
		///
		public static bool IsIdent(Char c) {
			return Char.IsLetterOrDigit(c) || c == '_';
		}
	}
	
}
