//using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class PointToMoveGesture : MonoBehaviour
{
    public OVRHand hand;
    public OVRSkeleton skeleton;
    public LayerMask surfaceMask;
    public AgentController agentController;

    public float maxDistance = 10f;
    public float holdTimeRequired = 0.35f;
    
    private float holdTimer;
 
    void Update()
    {
        if (!hand.IsTracked|| hand.HandConfidence != OVRHand.TrackingConfidence.High)
        {
            holdTimer = 0f;
            return;
        }

        if (!IndexExtended())
        {
            holdTimer = 0f;
            return;
        }

        Debug.Log("MOtion detected");
        holdTimer += Time.deltaTime;
        if (holdTimer < holdTimeRequired) return;

        /*
        Transform pointer = hand.PointerPose;
        if (pointer == null) return;
        */

        var wrist = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_Wrist].Transform;
        var tip = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip].Transform;
        Vector3 dir = (tip.position - wrist.position).normalized;

        if (Physics.Raycast(tip.position, dir, out RaycastHit hit, maxDistance, surfaceMask))
        {
            agentController.MoveTo(hit.point);
            holdTimer = 0f;
        }

    }

    bool IndexExtended()
    {

        var a = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexProximal].Transform.position;
        var b = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexIntermediate].Transform.position;
        var tip = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip].Transform.position;

        Debug.Log(Vector3.Dot((b - a).normalized, (tip - b).normalized) + " is less than " + 0.85f);
        return Vector3.Dot((b - a).normalized, (tip - b).normalized) >0.70f; //-0.70
        
    }

    //bool Pinching() => hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
}
