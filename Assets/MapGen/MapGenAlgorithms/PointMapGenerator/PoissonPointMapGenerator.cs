
using System.Collections.Generic;
using UnityEngine;

namespace PoissonPointMapGeneration
{
    public class PoissonPointMapGenerator
    {
        public List<Vector2> ResultPoints = new List<Vector2>();

        private int Radius;
        private Vector2 Center;
        private int MinDistance;                                    // Минимальное расстояние, на котором точки могут быть друг от друга
        private int PointsPerAttempt;                               // Сколько раз активная точка может попытаться поставить новую точку

        private List<Vector2> ActivePoints = new List<Vector2>();  // Массив активных точек
        //private int[,] grid;

        public PoissonPointMapGenerator(int radius, Vector2 center, int mindist, int pointperattemt)
        {
            Radius = radius;
            Center = center;

            MinDistance = mindist;
            PointsPerAttempt = pointperattemt;

            GeneratePoints();
            ResultPoints.Sort(CompareByY);
        }

        int CompareByY(Vector2 a, Vector2 b)
        {
            if (a.y > b.y)
                return 1;
            else if (a.y < b.y)
                return -1;
            else
                return 0;
        }

        void GeneratePoints()
        {
            //grid = new int[Radius, Radius]; // сетка

            Vector2 RndPos = new Vector2(Random.Range(-1 * Radius, Radius), Random.Range(-1 * Radius, Radius));
            AddPoint(RndPos);

            while (ActivePoints.Count > 0)
            {
                Vector2 actPoint = ActivePoints[Random.Range(0, ActivePoints.Count - 1)];

                for (int i = 0; i < PointsPerAttempt; i++)
                {
                    Vector2 newPoint = NewPointInDisc(actPoint);

                    if (FreeOfNeighbours(newPoint))
                        AddPoint(newPoint);
                }

                ActivePoints.Remove(actPoint);
            }
        }

        void AddPoint(Vector2 Point)
        {
            ActivePoints.Add(Point);
            ResultPoints.Add(Point);
        }

        Vector2 NewPointInDisc(Vector2 Point)
        {
            Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // Создадим рандомный вектор направления
            Vector2 NewPoint = Point + dir * Random.Range(MinDistance + 1, 2*MinDistance - 1);

            return NewPoint;
        }

        bool FreeOfNeighbours(Vector2 Point)
        {
            if (EnterOnCircle(Point, Center, Radius) && NoNeighboursTooClose(Point))
                return true;
            else
                return false;
        }

        bool EnterOnCircle(Vector2 p, Vector2 c, int r)
        {
            return ( (p.x - c.x) * (p.x - c.x) + (p.y - c.y) * (p.y - c.y) ) <= r * r;
        }

        bool NoNeighboursTooClose(Vector2 Point)
        {
            List<Vector2> PointsInCircle = ResultPoints.FindAll(p => EnterOnCircle(p, Point, MinDistance));

            if (PointsInCircle.Count == 0)
                return true;
            else
                return false;
        }

    }
}
