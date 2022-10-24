using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphStuff;

public class MapCore : MonoBehaviour
{
    [SerializeField] int Radius = 200;
    [SerializeField] int MinDistance = 40;               
    [SerializeField] int PointsPerAttempt = 30;           
    [SerializeField] int TotalIterToFindPath = 10;
    [SerializeField] int MaxPointDeactivationForIter = 10;
    [SerializeField] int MaxPointActivationForIter = 7;

    [HideInInspector] public GameObject CurMapObjectStorage;
    [HideInInspector] public GameObject Camera;

    Map CurrentMap;

    private void Start()
    {
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        DestroyCurrentMap();
        CurrentMap = new Map(Radius, MinDistance, PointsPerAttempt, TotalIterToFindPath, MaxPointDeactivationForIter, MaxPointActivationForIter);
        DisplayCurrentMap();
    }

    void DestroyCurrentMap()
    {
        if(CurrentMap != null)
        {
            foreach(Transform child in CurMapObjectStorage.transform)
            {
                Destroy(child.gameObject);
            }

            CurrentMap = null;
        }
    }

    void DisplayCurrentMap()
    {
        for (int i = 0; i < CurrentMap.LevelMap.NodesList.Count; i++)
        {
            Node curNode = CurrentMap.LevelMap.NodesList[i];

            for (int j = 0; j < curNode.Arcs.Count; j++)
            {
                Node chNode = curNode.Arcs[j].Child;
                DrawLine(curNode.Pos, chNode.Pos);
            }
        }

        Object DefaultLocPrefab = Resources.Load("Locations/Defaultlocation");

        foreach (Location el in CurrentMap.LocationList)
        {
            GameObject O = Instantiate(DefaultLocPrefab, el.NodeOnMap.Pos, Quaternion.identity, CurMapObjectStorage.transform) as GameObject;
            el.UnityObject = O.GetComponent<Location_VisualObject>();
            el.UnityObject.Init(el);
        }
    }

    void DrawLine(Vector2 pos1, Vector2 pos2)
    {
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();

        lineRenderer.transform.parent = CurMapObjectStorage.transform;

        lineRenderer.material.color = Color.black;
        lineRenderer.startWidth = 2.5f;
        lineRenderer.endWidth = 2.5f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
    }
}