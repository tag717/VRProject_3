using UnityEngine;
using Meta.XR.MRUtilityKit;

public class RoomReader : MonoBehaviour
{
    public AgentController agentController;
    public LayerMask surfaceLayer;

    private void Start() //used to be onEnable() but changed
    {
        if (MRUK.Instance == null) return;
        MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);

        if (MRUK.Instance.GetCurrentRoom() != null)
        {
            OnSceneLoaded();
        }
    }

    private void OnDestroy()
    {
        // Use OnDestroy to clean up the listener instead of OnDisable
        if (MRUK.Instance != null)
        {
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
        }
    }

    private void OnDisable()
    {
        if (MRUK.Instance != null)
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        if (room == null) return;

        var floor = room.FloorAnchor;
        if (floor != null)
        {
            floor.gameObject.layer = LayerMask.NameToLayer("Surface");
            EnsureCollider(floor);
        }
        foreach (var wall in room.WallAnchors)
        {
            wall.gameObject.layer = LayerMask.NameToLayer("Surface");
            EnsureCollider(wall);
        }

        foreach (var anchor in room.Anchors)
        {
            // 2. Check if this specific object's label is "TABLE"
            if (anchor.Label == MRUKAnchor.SceneLabels.TABLE)
            {
                anchor.gameObject.layer = LayerMask.NameToLayer("Obstracles");
                EnsureCollider(anchor);
            }
        }

        Debug.Log("Room loaded and surfaces mapped!");

        // 3. Bake the pure, naked floor FIRST
        if (agentController != null)
        {
            agentController.RebakeNavMesh();
            Debug.Log("NavMesh Bake Complete!");
        }
    }

    // Helper method to accurately size colliders based on the anchor data
    private void EnsureCollider(MRUKAnchor anchor)
    {
        if (anchor.gameObject.GetComponent<Collider>() == null)
        {
            if (anchor.HasPlane)
            {
                BoxCollider bc = anchor.gameObject.AddComponent<BoxCollider>();
                bc.size = new Vector3(anchor.PlaneRect.Value.width, anchor.PlaneRect.Value.height, 0.01f);
                bc.center = new Vector3(anchor.PlaneRect.Value.center.x, anchor.PlaneRect.Value.center.y, 0);
            }
            else if (anchor.HasVolume)
            {
                // We only need the BoxCollider now! The agent's Raycast will detect this.
                BoxCollider bc = anchor.gameObject.AddComponent<BoxCollider>();
                bc.size = anchor.VolumeBounds.Value.size;
                bc.center = anchor.VolumeBounds.Value.center;
            }
        }
    }
}