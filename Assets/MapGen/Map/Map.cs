using System.Collections.Generic;
using UnityEngine;
using DelaunayTriangulation;
using PoissonPointMapGeneration;
using GraphStuff;

public class Map
{
    [SerializeField] int Radius;
    [SerializeField] int MinDistance;                // Минимальное расстояние, на котором точки могут быть друг от друга
    [SerializeField] int PointsPerAttempt;           // Сколько раз можно попытаться поставить новую точку
    [SerializeField] int TotalIterToFindPath;

    [SerializeField] int MaxPointDeactivationForIter;
    [SerializeField] int MaxPointActivationForIter;

    public Graph LevelMap;
    public List<Location> LocationList = new List<Location>(); // Список локаций на карте

    Location StartLoc; // вероятно, не надо
    Location FinishLoc; // вероятно, не надо

    public Map(int R, int MinDist, int PPA, int TotalIter, int MaxDeactivation, int MaxActivation)
    {
        Radius = R;
        MinDistance = MinDist;
        PointsPerAttempt = PPA;
        TotalIterToFindPath = TotalIter;
        MaxPointDeactivationForIter = MaxDeactivation;
        MaxPointActivationForIter = MaxActivation;

        GenerateLevelMap();
        GenerateLocationOnMap();
    }

    //<summary>
    // Алгоритм создания карты состоит из нескольких шагов:
    // 1. Сгенерировать массив точек в пространстве (Poisson disk sampling)
    // 2. Соединить точки между собой (Delaunay triangulation)
    // 3. Создать связный граф из этих точек
    // 4. Найти кратчайший путь из старта к финишу по алгоритму Дийсктры (я определяю стартом наименьшую точка массива по оси У, а финиш как наибольшую)
    // 5. Исключить несколько точек из пути (По желанию: включить несколько уже исключенных)
    // 6. Повторить несколько раз п.4 и п.5
    // 7. Из получившихся путей сформировать итоговый граф
    //</summary>
    void GenerateLevelMap()
    {
        // Создаем карту точек
        PoissonPointMapGenerator PointGen = new PoissonPointMapGenerator(Radius, new Vector2(0, 0), MinDistance, PointsPerAttempt);
        List<Vector2> PointsList = PointGen.ResultPoints;

        // Подготовка к соединению точек
        List<IPoint> points = new List<IPoint>();

        for (int i = 0; i < PointsList.Count; i++)
            points.Add(new Point(PointsList[i].x, PointsList[i].y));

        // Соединяем точки
        Delaunator a = new Delaunator(points.ToArray());

        // Формируем из треугольников граф
        Graph MapGraph = new Graph();
        for (int i = 0; i < a.Points.Length; i++)
        {
            MapGraph.CreateNode(i, new Vector2((float)a.Points[i].X, (float)a.Points[i].Y));
        }

        // Связываем все вершины треугольника между собой
        List<Node> NodesInTriangle = new List<Node>();
        for (int i = 0; i < a.Triangles.Length; i++)
        {
            Node n = MapGraph.FindNode(a.Triangles[i]);
            NodesInTriangle.Add(n);

            if (NodesInTriangle.Count == 3)
            {
                for (int k = 0; k < 3; k++)
                {
                    Node curNode = NodesInTriangle[k];

                    for (int j = 0; j < 3; j++)
                    {
                        if (NodesInTriangle[j] != curNode)
                        {
                            curNode.AddArc(NodesInTriangle[j], (int)Get_Distance(curNode.Pos, NodesInTriangle[j].Pos));
                        }

                    }
                }

                NodesInTriangle.Clear();
            }
        }

        //Находим пути по графу
        List<List<Node>> PathList = new List<List<Node>>(); // массив путей
        Node StartNode = MapGraph.NodesList[0];
        Node FinishNode = MapGraph.NodesList[MapGraph.NodesList.Count - 1];

        // Неактивные точки не будут рассматриваться при нахождении пути
        List<Node> InactiveNodes = new List<Node>();

        for (int i = 0; i < TotalIterToFindPath; i++)
        {
            List<Node> Path = MapGraph.FindPathInGraph(StartNode.ID, FinishNode.ID); // Находим один из возможных путей до конца графа

            if (Path.Count > 0)
            {
                PathList.Add(Path);

                if (InactiveNodes.Count > 0)
                {
                    for (int r = 0; r < MaxPointActivationForIter; r++)
                    {
                        Node rndNode = InactiveNodes[Random.Range(0, InactiveNodes.Count - 1)];
                        rndNode.Activate();
                        InactiveNodes.Remove(rndNode);
                    }
                }

                for (int r = 0; r < MaxPointDeactivationForIter; r++)
                {
                    Node rndNode = Path[Random.Range(1, Path.Count - 1)];
                    rndNode.DeActivate();
                    InactiveNodes.Add(rndNode);
                }
            }
        }

        // Из получившихся путей создаем итоговый граф
        Graph ResultGraph = new Graph();
        int num = 0;

        ResultGraph.StartNode =  ResultGraph.CreateNode(0, StartNode.Pos);
        ResultGraph.FinishNode = ResultGraph.CreateNode(num++, FinishNode.Pos);

        for (int i = 0; i < PathList.Count; i++)
        {
            List<Node> CurPath = PathList[i];

            for (int j = 1; j < CurPath.Count; j++)
            {
                Node PrNode = CurPath[j - 1];
                Node CurNode = CurPath[j];

                Node PNode = ResultGraph.FindNode(PrNode.Pos);
                if (PNode is null)
                {
                    PNode = ResultGraph.CreateNode(num++, PrNode.Pos);
                }

                Node ChNode = ResultGraph.FindNode(CurNode.Pos);
                if (ChNode is null)
                {
                    ChNode = ResultGraph.CreateNode(num++, CurNode.Pos);
                }

                PNode.AddArc(ChNode, 1);
            }
        }

        // Можно заполнять MapGraph вместо создания нового графа
        LevelMap = ResultGraph;
    }

    void GenerateLocationOnMap()
    {
        List<Node> tempNodeList = new List<Node>();

        foreach(Node el in LevelMap.NodesList)
        {
            tempNodeList.Add(el);
        }

        for (int i = tempNodeList.Count - 1; i > 0; i--)
        {
            int v = Random.Range(0, i);
            Node tmp = tempNodeList[i];
            tempNodeList[i] = tempNodeList[v];
            tempNodeList[v] = tmp;
        }

        StartLoc = new Location_Start(LevelMap.StartNode);
        FinishLoc = new Location_Boss(LevelMap.FinishNode);
        
        LocationList.Add(StartLoc);
        LocationList.Add(FinishLoc);

        tempNodeList.Remove(LevelMap.StartNode);
        tempNodeList.Remove(LevelMap.FinishNode);

        int TotalLoc = tempNodeList.Count;
        int LootLoc = 3;
        int MiniBossLoc = 3;
        int EnemyLoc = TotalLoc - LootLoc - MiniBossLoc;

        for(int i = 0; i < LootLoc; i++)
        {
            LocationList.Add(new Location_Store(tempNodeList[0]));
            tempNodeList.Remove(tempNodeList[0]);
        }

        for (int i = 0; i < MiniBossLoc; i++)
        {
            LocationList.Add(new Location_EliteEnemy(tempNodeList[0]));
            tempNodeList.Remove(tempNodeList[0]);
        }

        for (int i = 0; i < EnemyLoc; i++)
        {
            LocationList.Add(new Location_Enemy(tempNodeList[0]));
            tempNodeList.Remove(tempNodeList[0]);
        }
    }

    float Get_Distance(Vector2 pos1, Vector2 pos2)
    {
        var heading = pos2 - pos1;
        var distance = heading.magnitude;

        return distance;
    }
}