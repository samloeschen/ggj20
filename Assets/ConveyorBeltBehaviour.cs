using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltBehaviour : MonoBehaviour {
    public GameObject[] prefabs;
    public Vector3 spawnOffset;
    public Vector3 partTargetOffset;
    public AnimationCurve partSpawnTranslationCurve;

    public float spawnPartDelay;
    public float partAnimationTime;
    float _delayTimer;
    float _partAnimTimer;

    public bool _addingPart;
    public PartBehaviour currentPart;

    public LayerMask clearCheckLayers;
    public Vector2 clearCheckOverlapSize;


    public ConveyorBeltBehaviour upConveyorBelt;
    public ConveyorBeltBehaviour downConveyorBelt;
    public ConveyorBeltBehaviour rightConveyorBelt;
    public ConveyorBeltBehaviour leftConveyorBelt;

    public Dictionary<Direction, ConveyorBeltBehaviour> adjacentConveyorBeltBehaviours;

    void Awake() {
        adjacentConveyorBeltBehaviours = new Dictionary<Direction, ConveyorBeltBehaviour> {
            { Direction.Up,     upConveyorBelt      },
            { Direction.Down,   downConveyorBelt    },
            { Direction.Right,  rightConveyorBelt   },
            { Direction.Left,   leftConveyorBelt    },
        };
    }

    public ConveyorBeltBehaviour GetAdjacent(Direction dir) {
        if (adjacentConveyorBeltBehaviours.TryGetValue(dir, out var conveyorBelt)) {
            return conveyorBelt?.currentPart != null ? conveyorBelt : null;
        }
        return null;
    }

    public void SpawnPart() {
        // grab random prefab and swap with last element in array to avoid duplicates
        var idx = Random.Range(0, prefabs.Length - 1);
        var prefab = prefabs[idx];
        prefabs[idx] = prefabs[prefabs.Length - 1];
        prefabs[prefabs.Length - 1] = prefab;

        // TODO randomize rotation?
        var clone = PoolManager.PoolInstantiate(prefab, transform.position + spawnOffset, Quaternion.identity);
        if (clone.TryGetComponent<PartBehaviour>(out currentPart)) {
            currentPart.rigidbody.isKinematic = true;
        }
        _delayTimer = spawnPartDelay;
    }

    public bool TryPickPart(out PartBehaviour part) {
        if (currentPart) {
            part = currentPart;
            part.Pick();
            currentPart = null;
            _partAnimTimer = 0f;
            GameManager.instance.SetActivePart(part);
            return true;
        }
        part = null;
        return false;
    }

    void Update() {
        if (currentPart) {
            _partAnimTimer += Time.deltaTime / partAnimationTime;
            _partAnimTimer = Mathf.Min(_partAnimTimer, 1f);
            currentPart.rigidbody.position = Vector3.Lerp(
                transform.position + spawnOffset,
                transform.position + partTargetOffset,
                partSpawnTranslationCurve.Evaluate(_partAnimTimer)
            ) + currentPart.spawnOffset;
            if (currentPart.partState == PartState.Picked) {
                currentPart = null;
            }
        }
        if (!currentPart) {
            // if (!Physics2D.OverlapBox(transform.position, clearCheckOverlapSize, 0f, clearCheckLayers)) {
                _delayTimer -= Time.deltaTime;
                if (_delayTimer <= 0f) {
                    SpawnPart();
                }
            // }
        }
    }
}

public enum Direction {
    Up, Down, Left, Right
}
