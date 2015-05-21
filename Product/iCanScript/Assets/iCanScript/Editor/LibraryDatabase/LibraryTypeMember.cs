using System.Reflection;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines a Unity event handler library object.
    public abstract class LibraryTypeMember : LibraryObject {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public bool isField          { get { return this is LibraryField; }}
        public bool isProperty       { get { return this is LibraryProperty; }}
        public bool isConstructor    { get { return this is LibraryConstructor; }}
        public bool isMethod         { get { return this is LibraryMemberInfo && (this as LibraryMemberInfo).memberType == MemberTypes.Method; }}
        public bool isPropertyGetter { get { return this is LibraryPropertyGetter; }}
        public bool isPropertySetter { get { return this is LibraryPropertySetter; }}
		
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryTypeMember() : base() {}

        // ======================================================================
        // Filtering Utilities.
        // ----------------------------------------------------------------------
		/// Updates the compunded score for this libray object.
        ///
        /// @param scoreProduct The product of the raw score the parent nodes.
        /// @param scoreSum The sum of the parent raw score.
        /// @param scoreSumLen The total number of character the filter is based on.
        ///
		public override void ComputeScore(float scoreProduct= 1f, float scoreSum= 0f, int scoreSumLen= 0) {
            // -- Update score components. --
            UpdateScoreComponents(ref scoreProduct, ref scoreSum, ref scoreSumLen);
			// -- Update our score if we are a member of a type. --
			if(this is LibraryTypeMember) {
                if(scoreSumLen == 0) {
                    myScore= 1f;
                }
                else {
                    myScore= scoreProduct * (scoreSum/scoreSumLen);
                }
			}
		}

        // ----------------------------------------------------------------------
		/// Computes the visibility of the library object.
        ///
        /// For type members, the visibility is affected by the "Show Inheritance"
        /// and "Show Protected" flags as well as the filtering score.
        ///
		public override void ComputeVisibility() {
            // -- Filter-out bad score. --
            var bestScore= this.libraryRoot.score;
			if(myScore < LibraryRoot.kMinScoreFactor * bestScore) {
                myIsVisible= false;
                return;
            }

			// -- Filter-out inherited members if requested by user. --
			var libraryMemberInfo= this as LibraryMemberInfo;
			if(libraryMemberInfo != null) {
	            if(this.libraryRoot.showInheritedMembers == false) {
	                if(libraryMemberInfo.isInherited) {
                        myIsVisible= false;
	            		return;
	            	}
				}
				// -- Should we show protected members? --
				if(this.libraryRoot.showProtectedMembers == false) {
					var methodBase= libraryMemberInfo.memberInfo as MethodBase;
					if(methodBase != null && methodBase.IsFamily) {
                        myIsVisible= false;
						return;
					}
				}				
			}
            this.libraryRoot.IncrementVisibleMemberCount();
			myIsVisible= true;
		}

    }
    
}
