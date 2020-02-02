using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillWall : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<PartBehaviour>(out var _)) {
            PoolManager.PoolDestroy(other.gameObject);
        }
    }
}
