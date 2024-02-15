using ImGuiNET;
using UnityEngine;

public static class Logger
{
    private const int MaxLogs = 1000;
    private static readonly string[] _logs = new string[MaxLogs];
    private static int _logIndex;

    /// <summary>
    /// Adds an entry to the log
    /// </summary>
    /// <param name="log">Message to write to the log</param>
    public static void Log(string log)
    {
        _logs[_logIndex] = log;
        _logIndex = (_logIndex + 1) % MaxLogs;
    }

    /// <summary>
    /// Draws the log header
    /// </summary>
    public static void Draw()
    {
        if (ImGui.CollapsingHeader("Log", ImGuiTreeNodeFlags.DefaultOpen))
        {
            float logHeight = Mathf.Max(ImGui.GetContentRegionAvail().y, ImGui.GetTextLineHeight() * 10);
            ImGui.BeginChild("LogScrollRegion", new Vector2(0, logHeight), false, ImGuiWindowFlags.HorizontalScrollbar);

            for (int i = 0; i < MaxLogs; i++)
            {
                int index = (_logIndex + i) % MaxLogs;
                if (_logs[index] != null)
                {
                    ImGui.TextUnformatted(_logs[index]);
                }
            }

            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            {
                ImGui.SetScrollHereY(1.0f);
            }

            ImGui.EndChild();
        }
    }
}
