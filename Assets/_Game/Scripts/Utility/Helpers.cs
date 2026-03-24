using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Helpers
{
    public static Vector3 ConvertToWorldPosition(Camera gameCamera, Camera uiCamera, Vector2 uiPosition)
    {
        // Convert UI position to world position
        Vector3 worldPosition = uiCamera.ScreenToWorldPoint(new Vector3(uiPosition.x, uiPosition.y, gameCamera.nearClipPlane));

        // Adjust the z-coordinate to match the game camera's position
        worldPosition.z = gameCamera.transform.position.z;

        return worldPosition;
    }
    
    public static bool IsPointerOverUIObject()
    {
        var eventSystem = EventSystem.current;
        if (!eventSystem) return false;
        if (eventSystem.IsPointerOverGameObject()) return true;
    
        var pointerEventData = new PointerEventData(eventSystem)    
        {
            position = Input.mousePosition
        };
    
        var results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);
    
        return results.Count > 0;
    }
}