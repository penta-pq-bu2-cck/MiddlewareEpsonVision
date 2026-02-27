using System;
using System.Collections.Generic;

namespace MiddlewareEpsonVision
{
    public class PointListHandler
    {
        public string BuildCommand(List<RobotPoint> points)
        {
            if (points == null || points.Count == 0)
                return string.Empty;

            List<string> result = new List<string>();
            int i = 0;

            while (i < points.Count)
            {
                int status = points[i].PointStatus;

                if (status == 0)
                {
                    result.Add($"P{i}:P{i}");
                    i++;
                }
                else if (status == 101 || status == 201)
                {
                    int start = i;
                    int expectedEnd = (status == 101) ? 102 : 202;

                    int j = i + 1;
                    bool found = false;

                    while (j < points.Count)
                    {
                        if (points[j].PointStatus == expectedEnd)
                        {
                            result.Add($"P{start}:P{j}");
                            i = j + 1;
                            found = true;
                            break;
                        }
                        j++;
                    }

                    if (!found)
                        throw new Exception($"Path starting at P{start} not closed");
                }
                else
                {
                    i++;
                }
            }

            return string.Join(",", result);
        }
    }
}