using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public new Transform transform;
    public Vector3 axis;
    public float speed;

    void OnValidate() {
        transform = GetComponent<Transform>();
    }

    void Update() {
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}
