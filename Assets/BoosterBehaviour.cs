using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterBehaviour : MonoBehaviour, IPartMovement {

    public new Rigidbody2D rigidbody;

    public float rocketForce;
    public float rocketTorque;
    public float maxTorque;
    public PartBehaviour partBehaviour;

    bool _fire;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        partBehaviour = GetComponent<PartBehaviour>();
        partBehaviour.onStateChange += (PartState state) => {
            switch (state) {
            case PartState.Wait:
                this.enabled = false;
                break;
            case PartState.Picked:
                this.enabled = true;
                rigidbody.isKinematic = false;
                break;
            case PartState.Stuck:
                this.enabled = false;
                break;
            case PartState.Dead:
                this.enabled = false;
                break;
            }
        };
        this.enabled = false;
    }

    public void Fire() {
        _fire = true;
    }
    void FixedUpdate() {
        if (Input.GetButton("Jump")) {
            _fire = true;
        }
        if (_fire) {
            rigidbody.AddForce(transform.up * rocketForce);
            _fire = false;
        }
        rigidbody.AddTorque(-Input.GetAxis("Horizontal") * rocketTorque);
        rigidbody.angularVelocity = Mathf.Max(Mathf.Abs(rigidbody.angularVelocity), maxTorque) * Mathf.Sign(rigidbody.angularVelocity);
    }
}
