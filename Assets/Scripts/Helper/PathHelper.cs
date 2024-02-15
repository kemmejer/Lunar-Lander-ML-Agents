using System.IO;
using UnityEngine;

public static class PathHelper
{
    /// <summary>
    /// Converts the specified relative path to work with the unity editor or the standalone application
    /// </summary>
    /// <param name="path">Path to be converted</param>
    /// <returns>Converted path</returns>
    public static string FixEditorOrBuildPath(string path)
    {
        if (Application.isEditor)
            return path;

        return Path.Combine("..", path);
    }
}

