using MiddlewareEpsonVision;
using System;
using System.Collections.Generic;

public class PointListHandler
{
    private static readonly Dictionary<int, int> PathStartToEndMap =
        new Dictionary<int, int>
    {
        { 101, 102 },
        { 201, 202 }
    };

    public string BuildCommand(List<RobotPoint> points)
    {
        if (points == null || points.Count == 0)
            return string.Empty;

        List<string> result = new List<string>();
        int i = 0;

        while (i < points.Count)
        {
            int status = points[i].PointStatus;

            // Normal point
            if (status == 0)
            {
                result.Add($"{i}:{i}");
                i++;
                continue;
            }

            // Path start detection
            if (PathStartToEndMap.TryGetValue(status, out int expectedEnd))
            {
                int start = i;
                int j = i + 1;
                bool found = false;

                while (j < points.Count)
                {
                    if (points[j].PointStatus == expectedEnd)
                    {
                        result.Add($"{start}:{j}");
                        i = j + 1;
                        found = true;
                        break;
                    }
                    j++;
                }

                if (!found)
                    throw new Exception($"Path starting at P{start} not closed");

                continue;
            }

            // Skip middle markers (100, 200, etc.)
            i++;
        }

        return string.Join(",", result);
    }
}