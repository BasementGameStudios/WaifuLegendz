using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Team))]
public class Tower : MonoBehaviour {

    public enum Lane { Mid };
    public Lane lane = Lane.Mid;
    public Team.Faction team;
    public Health nextStructure;

    private void Start()
    {
        team = GetComponent<Team>().faction;
    }

    public static int towerIndex = 1;

    private void OnDestroy()
    {
        if(nextStructure!=null)
            nextStructure.isAttackable = true;
    }
}


