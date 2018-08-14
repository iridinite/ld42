using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BuildProjectMenuItem : ScriptableObject {

	[MenuItem("Tools/Build All Targets")]
	private static void BuildAllPlatforms() {
		DoBuildPipeline(false);
	}

	[MenuItem("Tools/Build and Package All Targets")]
	private static void BuildAndZipAllPlatforms() {
		DoBuildPipeline(true);
	}

	private static void DoBuildPipeline(bool package) {
		if (!EditorUtility.DisplayDialog(GetGameName(), "Continue making builds for all targets?", "Build", "Cancel"))
			return;

		// determine build folder paths
		var locationDir = Application.dataPath + "/../Build/Staging/";
		var locationExe = GetGameName();
		// set up player build configuration
		var conf = new BuildPlayerOptions {
			options = /*BuildOptions.CompressWithLz4 |*/ BuildOptions.StrictMode,
			scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes)
		};

		// clear the staging area
		if (Directory.Exists(locationDir))
			Directory.Delete(locationDir, true);

		// create builds for each target platform
		//if (!CreateBuild(conf, BuildTarget.WebGL, "Web", locationExe, locationDir, package))
		//	return;
		//if (!CreateBuild(conf, BuildTarget.StandaloneLinuxUniversal, "Linux", locationExe, locationDir, package))
		//	return;
		//if (!CreateBuild(conf, BuildTarget.StandaloneOSX, "OSX", locationExe, locationDir, package))
		//	return;
		if (!CreateBuild(conf, BuildTarget.StandaloneWindows, "Win32", locationExe + ".exe", locationDir, package))
			return;
		if (!CreateBuild(conf, BuildTarget.StandaloneWindows64, "Win64", locationExe + ".exe", locationDir, package))
			return;

		// remove the staging directory
		//Directory.Delete(locationDir);
		Debug.Log("--- All builds finished ---");
	}

	private static string GetGameName() {
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(PlayerSettings.productName).Replace(" ", String.Empty);
	}

	private static bool CreateBuild(BuildPlayerOptions conf, BuildTarget target, string platform, string exename, string basedir, bool package) {
		var buildDir = basedir + platform + "/";
		conf.locationPathName = buildDir + exename;
		conf.target = target;

		// create the build
		Debug.Log(String.Format("Building {0}...", platform));
		var ret = BuildPipeline.BuildPlayer(conf);
		if (ret.summary.result != BuildResult.Succeeded) {
			Debug.LogError("Build Error: " + ret);
			return false;
		}

		// zip the build
		if (!package) return true;

		var proc = Process.Start(Application.dataPath + "/../Raw/7zip/7za.exe", String.Format("a -mx=9 -mmt8 -- {0}{1}-{2}.zip {3}*", basedir, GetGameName(), platform, buildDir));
		proc.WaitForExit();
		return true;
	}

}
