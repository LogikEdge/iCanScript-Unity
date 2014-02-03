using UnityEngine;
using System.Collections;

public static class iCS_WebConfig {
    // ======================================================================
    public const string URL                  = "www.icanscript.com";
	public const string ResourcesPath	     = URL+"/resources";
	public const string ReleasesPath		 = ResourcesPath+"/releases";
	public const string LatestReleasePath	 = ReleasesPath+"/latest";
	public const string LatestVersionInfoFile= LatestReleasePath+"/version-info.json";
	public const string SupportPath			 = URL+"/support";
	public const string DownloadsPage	     = SupportPath+"/downloads";
    public const string PerlScriptPath       = ResourcesPath+"/pl_scripts";
}
