using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodSpawner : MonoBehaviour {
    public GameObject[] podPrefabs;
    public Vector3 targetPodPos;
    public PodBehaviour currentPod;

    public AnimationCurve podTranslationCurve;
    public float translationAnimDuration;
    float _animT;

    bool _spawnAnimation;

    public Transform podParent;
    public float podParentStartZ;
    public float podParentTargetZ;
    public AnimationCurve podParentDeployCurve;
    public bool podDeployAnimActive;
    public float podParentDeployDuration;
    public float podKillDist;

    public int podDefaultLayer;
    public int podDeployLayer;
    float _podParentAnim;


    public FastList<PodBehaviour> exitingPods;


    void Awake() {
        podParent = new GameObject("PodParent").transform;
        exitingPods = new FastList<PodBehaviour>(8);
    }

    public void SpawnPod() {
        var idx = Random.Range(0, podPrefabs.Length - 1);
        var prefab = podPrefabs[idx];
        podPrefabs[idx] = podPrefabs[podPrefabs.Length - 1];
        podPrefabs[podPrefabs.Length - 1] = prefab;

        podParent.transform.position = new Vector3 { z = podParentStartZ };

        var clone = Instantiate(prefab, transform.position, Quaternion.identity);
        clone.transform.SetParent(podParent, worldPositionStays: true);
        if (clone.TryGetComponent<PodBehaviour>(out currentPod)) {
            currentPod.StopAnimation();
            currentPod.rigidbody.isKinematic = true;
        }
        _animT = 0f;
        _podParentAnim = 0f;
        _spawnAnimation = true;
        currentPod.SetLayer(podDefaultLayer);
    }

    public void DeployPod() {
        currentPod.rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        _podParentAnim = 0f;
        podDeployAnimActive = true;
        currentPod.BeginFlight();
        currentPod.SetLayer(podDeployLayer);
    }

    void Update() {
        if (_spawnAnimation) {
            _animT += Time.deltaTime / translationAnimDuration;
            currentPod.rigidbody.position = Vector3.LerpUnclamped(transform.position, targetPodPos, podTranslationCurve.Evaluate(_animT));
            if (_animT >= 1f) {
                currentPod.BeginIdleAnimation();
                _spawnAnimation = false;
            }
        }

        if (podDeployAnimActive) {
            if (_podParentAnim <= 1f) {
                _podParentAnim += Time.deltaTime / podParentDeployDuration;
                podParent.transform.position = new Vector3 {
                    z = Mathf.Lerp(podParentStartZ, podParentTargetZ, podParentDeployCurve.Evaluate(_podParentAnim))
                };
                if (_podParentAnim >= 1f) {
                    currentPod.transform.SetParent(null, worldPositionStays: true);
                    currentPod.rigidbody.constraints = RigidbodyConstraints2D.None;
                    exitingPods.Add(currentPod);
                    currentPod = null;
                    GameManager.instance.GoToWaitState();
                    podDeployAnimActive = false;
                }
            }
        }

        for (int i = 0; i < exitingPods.count; i++) {
            var pod = exitingPods[i];
            pod.FireParts();
            pod.rigidbody.AddTorque(Mathf.Sin(Time.time) * 500f * Time.fixedDeltaTime);
            var podXY = (Vector2)pod.transform.localPosition;
            var anchorXY = (Vector2)targetPodPos;
            var dist = (anchorXY - podXY).magnitude;
            if (dist > podKillDist) {
                pod.SetLayer(podDefaultLayer);
                pod.Kill();
                podDeployAnimActive = false;
                exitingPods.RemoveFast(i);
            }
        }
    }
}
