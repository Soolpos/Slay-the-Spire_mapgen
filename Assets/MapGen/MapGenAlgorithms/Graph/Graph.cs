using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GraphStuff
{
    public class Graph
    {
        public Node StartNode;
        public Node FinishNode;

        public List<Node> NodesList = new List<Node>();

        public Node CreateNode(int id, Vector2 pos)
        {
            var n = new Node(id, pos);
            NodesList.Add(n);
            return n;
        }

        public Node FindNode(int id)
        {
            var Result = NodesList.Find(x => x.ID == id);
            return Result;
        }

        public Node FindNode(Vector2 Pos)
        {
            var Result = NodesList.Find(x => x.Pos == Pos);
            return Result;
        }

        public int[,] CreateAdjMatrix()
        {
            int TotalNodes = NodesList.Count;

            int[,] AdjMatrix = new int[TotalNodes, TotalNodes]; // i - узел графа, j - расстояние от i до j

            for (int i = 0; i < TotalNodes; i++)
            {
                Node CN = NodesList[i];

                if (CN.IsActive())
                {
                    for (int j = 0; j < CN.Arcs.Count; j++)
                    {
                        Node ChildN = CN.Arcs[j].Child;

                        if (ChildN.IsActive())
                        {
                            int childnode = ChildN.ID;
                            AdjMatrix[i, childnode] = CN.Arcs[j].Weigth;
                        }
                    }
                }
            }

            return AdjMatrix;
        }

        public List<Node> FindPathInGraph(int StartNode, int FinishNode)
        {
            // Инициализация:
            // 1. Создаем матрицу смежности
            // 2. Создаем массив маркированных узлов
            // 3. Создаем массив расстояний от 0 до dist[v]

            List<Node> MinPath = new List<Node>();

            Node[] Nodes = NodesList.ToArray();
            int MaxValue = int.MaxValue;
            int TotalNodes = Nodes.Length;
            int[,] AdjMatrix = CreateAdjMatrix(); // i - узел графа, j - расстояние от i до j
            for (int i = 0; i < TotalNodes; i++)
            {
                Node CN = Nodes[i];
                if (CN.IsActive())
                {
                    for (int j = 0; j < CN.Arcs.Count; j++)
                    {
                        Node ChildN = CN.Arcs[j].Child;
                        if (ChildN.IsActive())
                        {
                            int childnode = ChildN.ID;
                            AdjMatrix[i, childnode] = CN.Arcs[j].Weigth;
                        }
                    }
                }
            }

            bool[] marked = new bool[TotalNodes];               // Массив помеченных вершин
            int[] dist = new int[TotalNodes];                   // Массив минимальной дистанции от 0 до остальных вершин
            int[] p = new int[TotalNodes];                      // Для каждой вершины v хранится номер вершины p[v], являющейся предпоследней в кратчайшем пути до вершины v


            for (int i = 0; i < TotalNodes; i++)                // Заполняем весь массив дистанций максимальным числом
                dist[i] = MaxValue;

            dist[StartNode] = 0;                                // Обозначаем стартовый узел

            for (int i = 0; i < TotalNodes; i++)                // Все узлы еще непомечены
                marked[i] = false;

            // Начало алгоритма
            for (int i = 0; i < TotalNodes; i++)
            {
                // Ищем непомеченный узел с минимальным весом
                int curnode = -1; // Выделенный узел в итерации
                int mind = MaxValue;
                for (int k = 0; k < TotalNodes; k++)
                {
                    if (!marked[k] && dist[k] < mind)
                    {
                        mind = dist[k];
                        curnode = k;
                    }
                }

                if (mind == MaxValue || curnode == -1)
                {
                    // алгоритм завершен, больше ничего сделать нельзя
                    //return MinPath;
                    continue;
                }

                for (int j = 0; j < TotalNodes; j++)
                {
                    if (AdjMatrix[curnode, j] > 0 && !marked[j])
                    {
                        int curdist = dist[curnode] + AdjMatrix[curnode, j];
                        if (curdist < dist[j])
                        {
                            // Улучшаем расстояние от вершины curnode до вершины j
                            dist[j] = curdist;
                            // Когда из выбранной вершины curnode происходит улучшение расстояния до вершины j, мы записываем, что предком вершины j является вершина curnode
                            p[j] = curnode;
                        }
                    }
                }

                marked[curnode] = true;
            }

            // Если добрались до финиша, то можно восстановить путь
            if (dist[FinishNode] < MaxValue)
            {
                for (int v = FinishNode; v != StartNode; v = p[v])
                {
                    MinPath.Add(Nodes[v]);
                }

                MinPath.Add(Nodes[0]);
            }

            MinPath.Reverse();
            return MinPath;
        }
    }

    public class Node // Узел графа
    {
        public int ID;
        public Vector2 Pos;
        public List<Arc> Arcs = new List<Arc>();
        bool Active = true;

        public Node(int id, Vector2 pos)
        {
            ID = id;
            Pos = pos;
        }

        public void AddArc(Node child, int w)
        {
            if(!Arcs.Exists(a => a.Parent == this && a.Child == child))
                Arcs.Add(new Arc(this, child, w));

            //if (!child.Arcs.Exists(a => a.Parent == child && a.Child == this))
            //    child.Arcs.Add(new Arc(child, this, w));
        }

        public bool IsActive()
        {
            return Active;
        }

        public void Activate()
        {
            Active = true;
        }

        public void DeActivate()
        {
            Active = false;
        }
    } 

    public class Arc // Дуга графа
    {
        public int Weigth;
        public Node Parent;
        public Node Child;

        public Arc(Node parent, Node child, int weight)
        {
            Parent = parent;
            Child = child;
            Weigth = weight;
        }
    }

}
