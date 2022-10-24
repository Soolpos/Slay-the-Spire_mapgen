using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_VisualObject : MonoBehaviour
{
    Location Master;

    public void Init(Location master)
    {
        Master = master;

        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(master.Logopath);
    }
}
