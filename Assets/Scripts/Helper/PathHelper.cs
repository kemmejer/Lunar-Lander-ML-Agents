using System.IO;
using UnityEngine;

public static class PathHelper
{
    public static string FixEditorOrBuildPath(string path)
    {
        if (Application.isEditor)
            return path;

        return Path.Combine("..", path);
    }
}

