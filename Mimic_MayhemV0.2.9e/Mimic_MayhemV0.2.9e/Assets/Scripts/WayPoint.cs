using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour {

    [SerializeField]
    protected float deBugDrawRadius = 1.0f;

    //Draws spheres at Waypoint Positions
    public virtual void OnDrawGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deBugDrawRadius);
    }
	
}
