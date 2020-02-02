using UnityEngine;
public class CameraBehaviour : MonoBehaviour {
    public float posSmoothing;
    public float zoomSmoothing;

    public Rect bounds;


    public CameraFocusData neutralFocusData;
    public CameraFocusTransform target;

    void FixedUpdate() {

        var data = target?.cameraFocusData ?? neutralFocusData;

        // position
        var cPos = new Vector2 { x = transform.position.x, y = transform.position.y };
        var tPos = new Vector2 { x = data.targetPos.x, y = data.targetPos.y };


        tPos.x = Mathf.Min(bounds.xMax, tPos.x);
        tPos.x = Mathf.Max(bounds.xMin, tPos.x);
        tPos.y = Mathf.Min(bounds.yMax, tPos.y);
        tPos.y = Mathf.Max(bounds.yMin, tPos.y);
        cPos = Vector2.Lerp(cPos, tPos, 1f - Mathf.Exp(-posSmoothing * Time.deltaTime));

        // zoom
        var cZoom = transform.position.z;
        cZoom = Mathf.Lerp(cZoom, data.targetPos.z - data.targetZoomDist, 1f - Mathf.Exp(-zoomSmoothing * Time.deltaTime));

        transform.position = new Vector3 {
            x = cPos.x,
            y = cPos.y,
            z = cZoom
        };
    }
}
