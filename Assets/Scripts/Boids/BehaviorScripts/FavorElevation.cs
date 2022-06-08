using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/FavorElevation")]
public class FavorElevation : FlockBehavior
{
    Vector3 currentVelocity;
    public float elevation = 125;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // newElevation = elevation + (25 * Mathf.Ceil( Mathf.Sin(Mathf.Deg2Rad * Time.frameCount * .5f)));

        float verticalOffset = elevation - agent.transform.position.y;

        Vector3 dir = new Vector3(0, verticalOffset , 0);
        // Debug.DrawLine(agent.transform.position, agent.transform.position + dir);
        if(verticalOffset > 0)
        {
            return dir;
        }
        return Vector3.zero;
    }
}

