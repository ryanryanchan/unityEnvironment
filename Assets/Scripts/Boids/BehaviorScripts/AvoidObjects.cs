using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/AvoidObjects")]
public class AvoidObjects : FlockBehavior
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 1f;
    public float collisionDistance = 50f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3[] directions = BoidHelper.rayDirections;
        for (int i = 0; i < BoidHelper.rayDirections.Length; i++)
        {
            Vector3 dir = agent.transform.TransformDirection(directions[i]);

            RaycastHit hit;

            Ray ray = new Ray(agent.transform.position, dir);
            if (!Physics.SphereCast(ray, 5f,collisionDistance, Physics.DefaultRaycastLayers))
            {
                // Debug.DrawLine(agent.transform.position, agent.transform.position + dir * collisionDistance);
                return dir;
            }
        }
        return Vector3.zero;
    }
}

