using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
//using System.Diagnostics;

public class AgentController : MonoBehaviour
{
    public NavMeshSurface surface;
    public NavMeshAgent agent;
    public Animator animator;

    public float sampleRadius = 1.0f;
    public float stoppingDistance = 0.2f;
    public void RebakeNavMesh()
    {
        surface.BuildNavMesh(); //called in RoomReader once the actual colliders are done
    }

    public void MoveTo(Vector3 rawTarget)
    {
        if (!agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void Start()
    {
        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }
    void Update()
    {
if (animator != null && agent != null)
        {
            //Cast a ray forward to detect furniture
            Vector3 chestHeight = transform.position + Vector3.up * 1.0f;
            int obstacleMask = LayerMask.GetMask("Obstacles");

            if (Physics.Raycast(chestHeight, transform.forward, out RaycastHit hit, 0.2f, obstacleMask))
            {
                // If the ray hits a table or couch, instantly stop the agent
                if (agent.hasPath)
                {
                    agent.ResetPath();
                }
            }

            // Normal walking logic
            bool isCloseEnough = agent.remainingDistance <= agent.stoppingDistance;
            bool walking = !isCloseEnough && agent.velocity.magnitude > 0.05f;
            animator.SetBool("Walking", walking);

            if (isCloseEnough && agent.hasPath)
            {
                agent.ResetPath(); 
            }
        }
    }


    private void OnAnimatorIK(int layerInde) 
    {
        if (animator == null) return;

        int surfaceMask = LayerMask.GetMask("Surface");
        //Left foot
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        var p = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        if (Physics.Raycast(p + Vector3.up * 0.5f, Vector3.down, out RaycastHit h, 1f, surfaceMask))
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, h.point);

        //right foot
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        var p2 = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        if (Physics.Raycast(p2 + Vector3.up * 0.5f, Vector3.down, out RaycastHit h2, 1f, surfaceMask))
            animator.SetIKPosition(AvatarIKGoal.RightFoot, h2.point);    
    }
}