using System;
using UnityEngine;

public static class CommandLineHelper
{
    public static string SelectedConfig = string.Empty;
    public static bool IsTrainingApplicationInstance;
    public static bool IsTrainingApplicationHost;

    /// <summary>
    /// Reads the command line arguments on startup and applies the specified configuration
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ReadCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        foreach (var arg in args)
        {
            if (arg.Contains("--selected-ml-config"))
            {
                var configArg = arg.Split('=');
                if (configArg.Length == 2)
                {
                    SelectedConfig = configArg[1];
                    IsTrainingApplicationInstance = !Application.isEditor;
                }
            }
        }

        IsTrainingApplicationHost = !Application.isEditor && !IsTrainingApplicationInstance;
    }
}
