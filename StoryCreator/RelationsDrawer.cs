using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading.Tasks;

namespace StoryCreator
{
    public static class RelationsDrawer
    {
        public static Polyline ConstructPolyline(PointF P0, PointF P1, PointF P2, PointF P3, bool isSelfLoopedRelation = false)
        {
            float p12Dist = (float)Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2));
            int divisionsNum = Math.Max((int)(p12Dist * .2f), 8);
            float tStep = 1f / (float)divisionsNum;
            P0.Y *= .5f;
            P3.Y *= 1.5f;
            Polyline newPolyLine = new System.Windows.Shapes.Polyline();
            PointCollection polylinePointsCollect = new PointCollection();
            if (isSelfLoopedRelation == false)
                BuildACurve(P0, P1, P2, P3, tStep, polylinePointsCollect);
            else
            {
                PointF P6 = P3;
                PointF P5 = P2;
                P3 = new PointF(P1.X - 160, P1.Y - 50);
                P2 = new PointF(P3.X + 35, P1.Y + 10f);
                PointF P4 = new PointF(P3.X + 35, P5.Y - 10f);
                BuildACurve(P0, P1, P2, P3, tStep, polylinePointsCollect);
                BuildACurve(P1, P2, P3, P4, tStep, polylinePointsCollect);
                BuildACurve(P2, P3, P4, P5, tStep, polylinePointsCollect);
                BuildACurve(P3, P4, P5, P6, tStep, polylinePointsCollect);
                /*
                for (float t = 0; t <= 1; t += tStep)
                {
                    if (t + tStep > 1) t = 1;
                    PointF newPF = CatmullRomEq(P0, P1, P2, P3, t);
                    polylinePointsCollect.Add(new System.Windows.Point((int)Math.Round(newPF.X), (int)Math.Round(newPF.Y)));
                }                                                      
                for (float t = 0; t <= 1; t += tStep)                  
                {                                                      
                    if (t + tStep > 1) t = 1;                          
                    PointF newPF = CatmullRomEq(P1, P2, P3, P4, t);    
                    polylinePointsCollect.Add(new System.Windows.Point((int)Math.Round(newPF.X), (int)Math.Round(newPF.Y)));
                }                                                      
                for (float t = 0; t <= 1; t += tStep)                  
                {                                                      
                    if (t + tStep > 1) t = 1;                          
                    PointF newPF = CatmullRomEq(P2, P3, P4, P5, t);    
                    polylinePointsCollect.Add(new System.Windows.Point((int)Math.Round(newPF.X), (int)Math.Round(newPF.Y)));
                }                                                      
                for (float t = 0; t <= 1; t += tStep)                  
                {                                                      
                    if (t + tStep > 1) t = 1;                          
                    PointF newPF = CatmullRomEq(P3, P4, P5, P6, t);     
                    polylinePointsCollect.Add(new System.Windows.Point((int)Math.Round(newPF.X), (int)Math.Round(newPF.Y)));
                }
                */
            }
            newPolyLine.Points = polylinePointsCollect;
            newPolyLine.Stroke = System.Windows.Media.Brushes.Black;
            newPolyLine.StrokeThickness = 2;
            return newPolyLine;
        }
        /*
        Polyline newPolyLine = new System.Windows.Shapes.Polyline();
    PointCollection myPointCollection2 = new PointCollection();
        return newPolyLine;
        */
        static void BuildACurve(PointF P0, PointF P1, PointF P2, PointF P3, float tStep, PointCollection polylinePointsCollect)
        {
            for (float t = 0; t <= 1; t += tStep)
            {
                if (t + tStep > 1) t = 1;
                PointF newPF = CatmullRomEq(P0, P1, P2, P3, t);
                polylinePointsCollect.Add(new System.Windows.Point((int)Math.Round(newPF.X), (int)Math.Round(newPF.Y)));
            }
        }
        static PointF CatmullRomEq(PointF P0, PointF P1, PointF P2, PointF P3, float t)
        {
            float p0Factor = -t * (1f - t) * (1f - t);
            float p1Factor = +(2f - 5f * t * t + 3f * t * t * t);
            float p2Factor = +t * (1f + 4f * t - 3f * t * t);
            float p3Factor = -t * t * (1f - t);

            float p0x = P0.X * p0Factor;
            float p0y = P0.Y * p0Factor;
            float p1x = P1.X * p1Factor;
            float p1y = P1.Y * p1Factor;
            float p2x = P2.X * p2Factor;
            float p2y = P2.Y * p2Factor;
            float p3x = P3.X * p3Factor;
            float p3y = P3.Y * p3Factor;

            float x = .5f * (p0x + p1x + p2x + p3x);
            float y = .5f * (p0y + p1y + p2y + p3y);

            return new PointF (x,y);
        }
    }
}
