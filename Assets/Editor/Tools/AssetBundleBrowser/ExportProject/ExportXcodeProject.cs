using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class ExportXcodeProject
{
    public static void ExportProject(string path)
    {
        path = path.Replace("\\", "/");

        BuildReport buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.AcceptExternalModificationsToPlayer);

        if (buildReport.summary.result == BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("ExportXcode", "Export xcode project success!", "ok");
        }
    }


    public static void ExportDebugProject(string path)
    {
        path = path.Replace("\\", "/");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = GetBuildScenes();

        buildPlayerOptions.locationPathName = path;

        buildPlayerOptions.target = BuildTarget.iOS;

        buildPlayerOptions.options = BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.ConnectWithProfiler;

        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (buildReport.summary.result == BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("ExportDebugXcode", "Export Debug xcode Project success", "ok");
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

}
