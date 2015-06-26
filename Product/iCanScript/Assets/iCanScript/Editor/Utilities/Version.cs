using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public class Version {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	public int MajorVersion;
    	public int MinorVersion;
    	public int BugFixVersion;
	
    	public static Version Current=
    		new Version(iCS_Config.MajorVersion,
    					    iCS_Config.MinorVersion,
    						iCS_Config.BugFixVersion);
	

        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
    	public Version(int major, int minor, int bugFix) {
    		MajorVersion = major;
    		MinorVersion = minor;
    		BugFixVersion= bugFix;
    	}

        // ======================================================================
        // Comparaisons
        // ----------------------------------------------------------------------
    	public bool IsEqual(Version other) {
    		return IsEqual(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
    	}
    	public bool IsEqual(int major, int minor, int bugFix) {
    		return MajorVersion  == major &&
    		       MinorVersion  == minor &&
    			   BugFixVersion == bugFix;
    	}
							
        // ----------------------------------------------------------------------
    	public bool IsNewerThen(Version other) {
    		return IsNewerThen(other.MajorVersion, other.MinorVersion, other.BugFixVersion);		
    	}
    	public bool IsNewerThen(int major, int minor, int bugFix) {
    		if(MajorVersion > major) return true;
    		if(MajorVersion < major) return false;
    		if(MinorVersion > minor) return true;
    		if(MinorVersion < minor) return false;
    		return BugFixVersion > bugFix;
    	}

        // ----------------------------------------------------------------------
    	public bool IsNewerOrEqualTo(Version other) {
    		return IsNewerOrEqualTo(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
    	}
    	public bool IsNewerOrEqualTo(int major, int minor, int bugFix) {
    		if(MajorVersion > major) return true;
    		if(MajorVersion < major) return false;
    		if(MinorVersion > minor) return true;
    		if(MinorVersion < minor) return false;
    		return BugFixVersion >= bugFix;
    	}
	
        // ----------------------------------------------------------------------
    	public bool IsOlderThen(Version other) {
    		return IsOlderThen(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
    	}
    	public bool IsOlderThen(int major, int minor, int bugFix) {
    		if(MajorVersion < major) return true;
    		if(MajorVersion > major) return false;
    		if(MinorVersion < minor) return true;
    		if(MinorVersion > minor) return false;
    		return BugFixVersion < bugFix;
    	}
        // ----------------------------------------------------------------------
    	public bool IsOlderOrEqualTo(Version other) {
    		return IsOlderOrEqualTo(other.MajorVersion, other.MinorVersion, other.BugFixVersion);
    	}
    	public bool IsOlderOrEqualTo(int major, int minor, int bugFix) {
    		if(MajorVersion < major) return true;
    		if(MajorVersion > major) return false;
    		if(MinorVersion < minor) return true;
    		if(MinorVersion > minor) return false;
    		return BugFixVersion <= bugFix;
    	}

        // ======================================================================
        // Common
        // ----------------------------------------------------------------------
    	// Serializes a version string.
    	public override string ToString() {
    		return MajorVersion.ToString()+"."+MinorVersion.ToString()+"."+BugFixVersion.ToString();
    	}
        // ----------------------------------------------------------------------
    	// Deserializes a version string.
    	public static Version FromString(string versionStr) {
    		int major = 0;
    		int minor = 0;
    		int bugFix= 0;
    		int idx= versionStr.IndexOf('.');
    		if(idx >= 1) {
    			string majorStr= versionStr.Substring(0, idx);
    			versionStr= versionStr.Substring(idx+1);		
    			major = (int)Convert.ChangeType(majorStr, typeof(int));
    			idx= versionStr.IndexOf('.');
    			if(idx >= 1) {
    				string minorStr= versionStr.Substring(0, idx);
    				minor = (int)Convert.ChangeType(minorStr, typeof(int));
    				versionStr= versionStr.Substring(idx+1);		
    				if(versionStr.Length > 0) {
    					bugFix= (int)Convert.ChangeType(versionStr, typeof(int));												
    				}
    			}
    		}
    		return new Version(major, minor, bugFix);
    	}
    }

}
