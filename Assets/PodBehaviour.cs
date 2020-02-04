using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodBehaviour : MonoBehaviour {

    public new Rigidbody2D rigidbody;

    public float returnDelay = 0.2f;
    float _delayTimer;

    public bool animatePosition;

    public float xFrequency;
    public float xMangitude;
    public float yFrequency;
    public float yMagnitude;

    [Header("Flight")]
    public float zMoveSpeed;
    public float zTarget;
    bool flightActive = false;



    public Vector3 anchorPos;
    float _magT;
    float _seed;


    public FastList<PartBehaviour> stuckParts;


    void OnValidate() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Awake() {
        stuckParts = new FastList<PartBehaviour>(128);
    }

    void OnEnable() {
        _seed = Random.value * 100f;
        stuckParts.Clear();
        rigidbody.isKinematic = true;
        flightActive = false;
    }

    void Update() {

        if (animatePosition) {
            _magT = Mathf.MoveTowards(_magT, 1f, Time.deltaTime * 2f);

            var t = Time.time + _seed;
            rigidbody.position = anchorPos + new Vector3 {
                x = Mathf.Sin(t * xFrequency) * xMangitude * _magT,
                y = Mathf.Cos(t * yFrequency) * yMagnitude * _magT
            };

            rigidbody.rotation = Mathf.Sin(Time.time * 0.5f + _seed) * _magT * 10f;
        }

        if (_delayTimer > 0f) {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer < 0f) {
                GameManager.instance.AbandonPart();
            }
        }

        if (flightActive) {
            var z = transform.position.z;
            z = Mathf.MoveTowards(z, zTarget, Time.deltaTime * zMoveSpeed);
            transform.position = new Vector3 {
                x = transform.position.x,
                y = transform.position.y,
                z = z
            };
        }

        for (int i = 0; i < stuckParts.count; i++) {
            var p = stuckParts[i].transform.position;
            p.z = transform.position.z;
            stuckParts[i].transform.position = p;
        }
    }

    public void FireParts() {
        for (int i = 0; i < stuckParts.count; i++) {
            var pm = stuckParts[i].GetComponent<IPartMovement>();
            if (pm != null) {
                pm.Fire();
            }
        }
    }

    public void SetLayer(int layer) {
        this.gameObject.layer = layer;
        for (int i = 0; i < stuckParts.count; i++) {
            stuckParts[i].gameObject.layer = layer;
        }
    }

    public void BeginIdleAnimation() {
        anchorPos = transform.position;
        animatePosition = true;
    }

    public void StopAnimation() {
        animatePosition = false;
    }

    public void BeginFlight() {
        rigidbody.isKinematic = false;
        flightActive = false;
        animatePosition = false;
        for (int i = 0; i < stuckParts.count; i++) {
            var part = stuckParts[i];
            part.rigidbody.gravityScale = 0f;
            if (part.TryGetComponent<IPartMovement>(out var movement)) {
                var mb = movement as MonoBehaviour;
                if (mb) {
                    mb.enabled = true;
                }
            }
        }
    }

    public void Kill() {
        for (int i = 0; i < stuckParts.count; i++) {
            PoolManager.PoolDestroy(stuckParts[i].gameObject);
        }
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        var gameObj = collision.gameObject;
        if (gameObj.TryGetComponent<PartBehaviour>(out var partBehaviour)) {
            partBehaviour.Stick(this.rigidbody);
            stuckParts.Add(partBehaviour);
            _delayTimer = returnDelay;
        }
    }
}
