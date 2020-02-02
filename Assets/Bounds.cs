using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<PartBehaviour>(out var partBehaviour)) {
            if (partBehaviour.GetInstanceID() == GameManager.instance.activePart.GetInstanceID()) {
                GameManager.instance.AbandonPart();
            }
        }
    }
}
