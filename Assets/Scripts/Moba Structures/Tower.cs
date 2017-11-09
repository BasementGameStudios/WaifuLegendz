using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Team))]
public class Tower : MonoBehaviour {

    public enum Lane { Mid };
    public Lane lane = Lane.Mid;
    public Team.Faction team;

    private void Start()
    {
        team = GetComponent<Team>().faction;
    }
}
