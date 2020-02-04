using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPickerManager : MonoBehaviour {

    public CameraBehaviour cameraBehaviour;
    public ConveyorBeltBehaviour startConveyorBelt;
    public ConveyorBeltBehaviour currentConveyorBelt;


    public Transform highlightTransform;
    public float highlightSmoothing;
    public Vector3 highlightOffset;
    public float highlightZ;


    void Start() {
        currentConveyorBelt = startConveyorBelt;
    }

    void Update() {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");

        ConveyorBeltBehaviour next = null;
        if (xInput > 0) {
            next = currentConveyorBelt.GetAdjacent(Direction.Right);
        }
        else if (xInput < 0) {
            next = currentConveyorBelt.GetAdjacent(Direction.Left);
        }

        if (yInput > 0) {
            next = currentConveyorBelt.GetAdjacent(Direction.Up);
        }
        else if (yInput < 0) {
            next = currentConveyorBelt.GetAdjacent(Direction.Down);
        }

        if (next && next != currentConveyorBelt) {
            GameManager.instance.musicManager.PlayToggleSound();
        }

        currentConveyorBelt = next ?? currentConveyorBelt;
        var cpos = currentConveyorBelt.transform.position + highlightOffset;
        cpos.z = highlightZ;

        highlightTransform.position = Vector3.Lerp(
            highlightTransform.position,
            cpos,
            1f - Mathf.Exp(-highlightSmoothing * Time.deltaTime)
        );

        if (Input.GetButtonDown("Jump")) {
            if (currentConveyorBelt.TryPickPart(out var part)) {
                if (part.TryGetComponent<CameraFocusTransform>(out var cameraFocusTransform)) {
                    cameraBehaviour.target = cameraFocusTransform;
                    this.enabled = false;
                    GameManager.instance.musicManager.PlaySelectSound();
                } else {
                    // do pick fail animation/sound
                }
            }
        }
    }
}
