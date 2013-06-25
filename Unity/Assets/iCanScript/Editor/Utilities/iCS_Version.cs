using UnityEngine;
using System.Collections;

public class iCS_Version {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public int MajorVersion;
	public int MinorVersion;
	public int BugFixVersion;
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	public iCS_Version(int major, int minor, int bugFix) {
		MajorVersion = major;
		MinorVersion = minor;
		BugFixVersion= bugFix;
	}

    // ======================================================================
    // Comparaisons
    // ----------------------------------------------------------------------
	public bool IsEqual(iCS_Version other) {
		return IsEqual(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
	}
	public bool IsEqual(int major, int minor, int bugFix) {
		return MajorVersion  == major &&
		       MinorVersion  == minor &&
			   BugFixVersion == bugFix;
	}
							
    // ----------------------------------------------------------------------
	public bool IsNewerThen(iCS_Version other) {
		return IsNewerThen(other.MajorVersion, other.MinorVersion, other.BugFixVersion);		
	}
	public bool IsNewerThen(int major, int minor, int bugFix) {
		if(MajorVersion < major) return true;
		if(MinorVersion < minor) return true;
		return BugFixVersion < bugFix;
	}

    // ----------------------------------------------------------------------
	public bool IsOlderThen(iCS_Version other) {
		return IsOlderThen(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
	}
	public bool IsOlderThen(int major, int minor, int bugFix) {
		if(MajorVersion > major) return true;
		if(MinorVersion > minor) return true;
		return BugFixVersion > bugFix;
	}

    // ======================================================================
    // Common
    // ----------------------------------------------------------------------
	public override string ToString() {
		return MajorVersion.ToString()+"."+MinorVersion.ToString()+"."+BugFixVersion.ToString();
	}
}
