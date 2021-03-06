using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class Alignment : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
            return Vector3.zero;

        Vector3 alignmentMove = Vector3.zero;

        foreach (Transform item in context)
        {
            alignmentMove += item.transform.forward;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
