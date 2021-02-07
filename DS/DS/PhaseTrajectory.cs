using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using DS.Helpers;
using DS.MathStructures.Points;
using DS.Models;

namespace DS
{
    public static class PhaseTrajectory
    {
        public static List<PointX> Get(BaseModel baseModel, PointX start, int skipCount, int getCount)
        {
            var current = start;
            var points = new List<PointX>();

            for (var i = 0; i < skipCount + getCount; i++)
            {
                var next = baseModel.GetNextPoint(current);

                if (next.TendsToInfinity(baseModel.AbsInf))
                    return new List<PointX> { PointX.Infinity };

                if (i >= skipCount)
                    points.Add(next);

                current = next;
            }

            return points;
        }

        public static List<PointX> GetWhileNotConnect(BaseModel baseModel, PointX start, int skipCount, double eps, int k = 1)
        {
            var points = new List<PointX>();
            var current = start;

            for (var i = 0; i < skipCount; i++)
                current = baseModel.GetNextPoint(current);

            start = baseModel.GetNextPoint(current);
            points.Add(start);
            current = start;

            do
            {
                for (var i = 0; i < k; i++)
                    current = baseModel.GetNextPoint(current);

                if (points.Count > 800000)
                    return new List<PointX>();

                if (current.TendsToInfinity(baseModel.AbsInf))
                    return new List<PointX> { PointX.Infinity };

                points.Add(current);
            } while (!current.AlmostEquals(start, eps));

            return points;
        }

        public static List<double[]> Get(BaseNModel baseModel, double[] start, int skipCount, int getCount)
        {
            var current = start;
            var points = new List<double[]>();

            for (var i = 0; i < skipCount + getCount; i++)
            {
                var next = baseModel.GetNextPoint(current);

                if (next.TendsToValue(baseModel.AbsInf))
                    return new List<double[]> { Vector.Create(baseModel.N, double.MaxValue) };

                if (i >= skipCount)
                    points.Add(next);

                current = next;
            }

            return points;
        }
    }
}
