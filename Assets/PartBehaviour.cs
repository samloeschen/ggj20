using System;
using UnityEngine;

public class PartBehaviour : MonoBehaviour {
    public PartState partState;
    public new Rigidbody2D rigidbody;
    public event Action<PartState> onStateChange;
    public Vector3 spawnOffset;

    public FixedJoint2D fixedJoint;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        fixedJoint = GetComponent<FixedJoint2D>();
    }
    void OnEnable() {
        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
        partState = PartState.Wait;
        if (onStateChange != null) {
            onStateChange(partState);
        }
        rigidbody.gravityScale = 1f;
    }

    void OnDisable() {
        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
    }

    public void Pick() {
        partState = PartState.Picked;
        if (onStateChange != null) {
            onStateChange(partState);
        }
    }

    public void Die() {
        partState = PartState.Dead;
        if (onStateChange != null) {
            onStateChange(partState);
        }
    }

    public void Stick(Rigidbody2D body) {
        partState = PartState.Stuck;
        if (onStateChange != null) {
            onStateChange(partState);
        }
        fixedJoint.enabled = true;
        fixedJoint.connectedBody = body;
        GameManager.instance.musicManager.PlayAttachSound();
        GameManager.instance.musicManager.PlayShout();
    }
}

public enum PartState {
    Wait, Picked, Stuck, Dead
}
