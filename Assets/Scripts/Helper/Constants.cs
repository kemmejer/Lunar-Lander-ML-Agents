
public static class Constants
{
    public const float MaxShipAngle = 90.0f;

    public static string ConfigPath = PathHelper.FixEditorOrBuildPath("config/parameter");
    public static string TrainingBatPath = PathHelper.FixEditorOrBuildPath("train.bat");
    public static string ModelPath = PathHelper.FixEditorOrBuildPath("results");

    public const string DefaultConfigName = "Default";
    public const string AgentName = "ShipAgent";
}
