using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

class BuildUtility
{
    private static void WindowsDevBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.StandaloneWindows64, BuildOptions.Development);
    }

    private static void MacOSDevBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.StandaloneOSX, BuildOptions.Development);
    }

    private static void AndroidDevBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.Android, BuildOptions.Development);
    }

    private static void WebGLDevBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.WebGL, BuildOptions.Development);
    }

    private static void IOSDevBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.iOS, BuildOptions.Development);
    }

    private static void WindowsBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.StandaloneWindows64, 0);
    }

    private static void MacOSBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.StandaloneOSX, 0);
    }

    private static void AndroidBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.Android, 0);
    }

    private static void WebGLBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.WebGL, 0);
    }

    private static void IOSBuilder()
    {
        string outputDir = GetArg("-customBuildPath");
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputDir, BuildTarget.iOS, 0);
    }

    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
