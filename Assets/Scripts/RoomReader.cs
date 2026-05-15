using UnityEngine;
using Meta.XR.MRUtilityKit;

public class RoomReader : MonoBehaviour
{
    public LayerMask surfaceLayer;

    private void OnEnable()
    {
        MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }

    private void OnDisable()
    {
        if (MRUK.Instance != null)
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        var room = MRUK.Instance.GetCurrentRoom();

        var floor = room.FloorAnchor;
        floor.gameObject.layer = LayerMask.NameToLayer("Surface");

        foreach (var wall in room.WallAnchors)
        {
            wall.gameObject.layer = LayerMask.NameToLayer("Surface");

            if (wall.gameObject.GetComponent<Collider>() == null)
                wall.gameObject.AddComponent<BoxCollider>();
        }
    }
}