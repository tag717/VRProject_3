using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class AgentController : MonoBehaviour
{
    public NavMeshSurface surface;
    public NavMeshAgent agent;
    public Animator animator;

    public float sampleRadius = 1.0f;

    public void RebakeNavMesh()
    {
        surface.BuildNavMesh();
    }

    public void MoveTo(Vector3 rawTarget)
    {
        if (!agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            bool walking = agent.velocity.magnitude > 0.05f;
            animator.SetBool("Walking", walking);
        }
    }
}