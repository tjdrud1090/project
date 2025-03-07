﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Process_Page.ToothTemplate.Utils
{
    class InterpolationUtils
    {

        public class BezierCurveSegment
        {
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }
            public Point FirstControlPoint { get; set; }
            public Point SecondControlPoint { get; set; }
        }

        public static List<BezierCurveSegment> InterpolatePointWithBezierCurves(List<Point> points, bool isClosedCurve)
        {
            if (points.Count < 3)
                return null;
            var toRet = new List<BezierCurveSegment>();

            // if is close curve then add the first point at the end
            if (isClosedCurve)
                points.Add(points.First());

            for (int i = 0; i < points.Count - 1; i++)
            {
                double x1 = points[i].X;
                double y1 = points[i].Y;

                double x2 = points[i + 1].X;
                double y2 = points[i + 1].Y;

                double x0, y0;
                if (i == 0)     // if is first point
                {
                    var previousPoint = points[points.Count - 2];
                    x0 = previousPoint.X;
                    y0 = previousPoint.Y;

                    #region isClosedCurve 여부
                    /* isClosedCurve 설정 풀어놓을 경우 사용 */
                    //if (isClosedCurve)
                    //{
                    //    var previousPoint = points[points.Count - 2];    // last Point, but one(due inserted the first at the end)
                    //    x0 = previousPoint.X;
                    //    y0 = previousPoint.Y;
                    //}
                    //else
                    //{
                    //    var previousPoint = points[i];      // if is the first point the previous one will be itself
                    //    x0 = previousPoint.X;
                    //    y0 = previousPoint.Y;
                    //}
                    #endregion
                }
                else
                {
                    // Previous Point
                    x0 = points[i - 1].X;
                    y0 = points[i - 1].Y;
                }

                double x3, y3;
                if (i == points.Count - 2)
                {
                    var nextPoint = points[1];  //second Point(due inserted the first at the end)
                    x3 = nextPoint.X;
                    y3 = nextPoint.Y;

                    #region isClosedCurve 여부
                    //if (isClosedCurve)
                    //{
                    //    var nextPoint = points[1];  //second Point(due inserted the first at the end)
                    //    x3 = nextPoint.X;
                    //    y3 = nextPoint.Y;
                    //}
                    //else    //Get some next point
                    //{
                    //    var nextPoint = points[i + 1];  //if is the last point the next point will be the last one
                    //    x3 = nextPoint.X;
                    //    y3 = nextPoint.Y;
                    //}
                    #endregion
                }
                else
                {
                    x3 = points[i + 2].X;
                    y3 = points[i + 2].Y;
                }

                // control points
                double xc1 = (x0 + x1) / 2.0;
                double yc1 = (y0 + y1) / 2.0; ;
                double xc2 = (x1 + x2) / 2.0;
                double yc2 = (y1 + y2) / 2.0; ;
                double xc3 = (x2 + x3) / 2.0;
                double yc3 = (y2 + y3) / 2.0; ;

                double len1 = Math.Sqrt(Math.Pow((x1 - x0), 2.0) + Math.Pow((y1 - y0), 2.0));
                double len2 = Math.Sqrt(Math.Pow((x2 - x1), 2.0) + Math.Pow((y2 - y1), 2.0));
                double len3 = Math.Sqrt(Math.Pow((x3 - x2), 2.0) + Math.Pow((y3 - y2), 2.0));

                double k1 = len1 / (len1 + len2);
                double k2 = len2 / (len2 + len3);

                double xm1 = xc1 + (xc2 - xc1) * k1;
                double ym1 = yc1 + (yc2 - yc1) * k1;

                double xm2 = xc2 + (xc3 - xc2) * k2;
                double ym2 = yc2 + (yc3 - yc2) * k2;

                const double smoothValue = 0.8;
                // Resulting control points. Here smoothValue is metioned
                // above coefficient K whose value should be in range[0...1].
                double ctrl1_x = xm1 + (xc2 - xm1) * smoothValue + x1 - xm1;
                double ctrl1_y = ym1 + (yc2 - ym1) * smoothValue + y1 - ym1;

                double ctrl2_x = xm2 + (xc2 - xm2) * smoothValue + x2 - xm2;
                double ctrl2_y = ym2 + (yc2 - ym2) * smoothValue + y2 - ym2;
                toRet.Add(new BezierCurveSegment
                {
                    StartPoint = new Point(x1, y1),
                    EndPoint = new Point(x2, y2),
                    //FirstControlPoint = i == 0 ? new Point(x1, y1) : new Point(ctrl1_x, ctrl1_y),
                    //SecondControlPoint = i == points.Count - 2 ? new Point(x2, y2) : new Point(ctrl2_x, ctrl2_y)

                    #region isClosedCurve 여부
                    FirstControlPoint = i == 0 && !isClosedCurve ? new Point(x1, y1) : new Point(ctrl1_x, ctrl1_y),
                    SecondControlPoint = i == points.Count - 2 && !isClosedCurve ? new Point(x2, y2) : new Point(ctrl2_x, ctrl2_y)
                    #endregion
                });
            }

            return toRet;
        }
    }
}
