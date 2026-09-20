using System;
using System.Collections.Generic;

namespace Lab1_Lissajous.Models;

public readonly struct ModelPoint
{
    public double X { get; }
    public double Y { get; }

    public ModelPoint(double x, double y)
    {
        X = x;
        Y = y;
    }
}

public static class LissajousModel
{
    public static List<ModelPoint> Generate(
        double ax,
        double ay,
        double fx,
        double fy,
        double phaseXDegrees,
        double phaseYDegrees,
        double dt)
    {
        double phaseX = phaseXDegrees * Math.PI / 180.0;
        double phaseY = phaseYDegrees * Math.PI / 180.0;

        double duration = GetDuration(fx, fy);
        int count = Math.Max(100, (int)Math.Ceiling(duration / dt) + 1);
        count = Math.Min(count, 10000);

        var points = new List<ModelPoint>(count);

        for (int i = 0; i < count; i++)
        {
            double t = i * duration / (count - 1);

            double x = ax * Math.Sin(2 * Math.PI * fx * t + phaseX);
            double y = ay * Math.Sin(2 * Math.PI * fy * t + phaseY);

            points.Add(new ModelPoint(x, y));
        }

        return points;
    }

    private static double GetDuration(double fx, double fy)
    {
        if (IsNearInteger(fx) && IsNearInteger(fy))
        {
            int a = Math.Max(1, (int)Math.Round(fx));
            int b = Math.Max(1, (int)Math.Round(fy));
            int gcd = Gcd(a, b);
            return 1.0 / gcd;
        }

        return 4.0 * Math.Max(1.0 / fx, 1.0 / fy);
    }

    private static bool IsNearInteger(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 0.0001;
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int temp = a % b;
            a = b;
            b = temp;
        }

        return Math.Abs(a);
    }
}
