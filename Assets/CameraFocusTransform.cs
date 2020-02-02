using UnityEngine;
public class CameraFocusTransform : MonoBehaviour {
    public CameraFocusData cameraFocusData;
    void Update() {
        cameraFocusData.targetPos = new Vector2 { x = transform.position.x, y = transform.position.y };
    }
}

[System.Serializable]
public struct CameraFocusData {
    public Vector3 targetPos;
    public float targetZoomDist;
}
