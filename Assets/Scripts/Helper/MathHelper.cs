public static class MathHelper
{
    public static float MapInRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
    }
}
