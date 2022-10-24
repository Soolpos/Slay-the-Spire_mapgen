using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GraphStuff;

public class Location
{
    public string ID;
    public string Logopath;
    public Node NodeOnMap;
    public Location_VisualObject UnityObject;

    public Location(Node n)
    {
        ID = Guid.NewGuid().ToString();
        NodeOnMap = n;
    }
}

public class Location_Boss : Location
{
    public Location_Boss(Node n) : base(n)
    {
        Logopath = "Locations/Boss";
    }
}

public class Location_Start : Location
{
    public Location_Start(Node n) : base(n)
    {
        Logopath = "Locations/Rest";
    }
}

public class Location_Enemy : Location
{
    public Location_Enemy(Node n) : base(n)
    {
        Logopath = "Locations/Battle";
    }
}

public class Location_EliteEnemy : Location
{
    public Location_EliteEnemy(Node n) : base(n)
    {
        Logopath = "Locations/MiniBoss";
    }
}

public class Location_Store : Location
{
    public Location_Store(Node n) : base(n)
    {
        Logopath = "Locations/Loot";
    }
}