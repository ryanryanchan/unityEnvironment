using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/StayInRadius")]
public class StayInRadius : FlockBehavior
{
    public Transform center;
    public float radius = 500f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 centerOffset = center.position - agent.transform.position;
        float t = centerOffset.magnitude / radius;
        if(t < 0.9f)
        {
            return Vector3.zero;
        }
        return centerOffset * t * t;

    }
}
