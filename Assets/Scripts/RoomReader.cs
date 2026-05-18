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

            if (wall.gameObject.GetComponent<Collider>() == null)
                wall.gameObject.AddComponent<BoxCollider>();
                EnsureCollider(wall);
        }
        Debug.Log("Room loaded and surfaces mapped!");

        //re-bake the NavMesh with the rendered floor colliders
        if (agentController != null)
        {
            agentController.RebakeNavMesh();
            Debug.Log("NavMesh Bake Complete!");
        }
    }

    // Helper method to accurately size colliders based on the anchor data
    private void EnsureCollider(MRUKAnchor anchor)
    {
        // Only add a collider if one doesn't exist
        if (anchor.gameObject.GetComponent<Collider>() == null)
        {
            // Walls and floors use 2D planes in the Scene API
            if (anchor.HasPlane)
            {
                BoxCollider bc = anchor.gameObject.AddComponent<BoxCollider>();
                
                // Scale the collider to match the physical plane's dimensions
                // We make the Z-axis (thickness) 0.01f so it acts as a solid surface
                bc.size = new Vector3(anchor.PlaneRect.Value.width, anchor.PlaneRect.Value.height, 0.01f);
                
                // Center the collider on the plane
                bc.center = new Vector3(anchor.PlaneRect.Value.center.x, anchor.PlaneRect.Value.center.y, 0);
            }
        }
    }
}