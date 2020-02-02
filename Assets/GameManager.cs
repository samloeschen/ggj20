using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public static GameManager instance;
    public PartPickerManager partPickerManager;
    public CameraBehaviour cameraBehaviour;
    public Timer timer;
    public PodSpawner podSpawner;

    public PartBehaviour activePart;
    public PodBehaviour activePod;


    public float podKillDist;

    public AnimationCurve podDeployCurve;
    public float podDeployDuration;
    public float podDeployTargetZ;

    float _podDeployAnimTimer;
    float _startZ;



    public bool waitingForGameStart;
    public bool waitingForGameEnd;

    void Awake() {
        instance = this;
        partPickerManager = FindObjectOfType<PartPickerManager>();
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        timer = FindObjectOfType<Timer>();
        podSpawner = FindObjectOfType<PodSpawner>();
    }

    void OnEnable() {
        waitingForGameStart = true;
        partPickerManager.enabled = false;
        timer.enabled = false;
    }

    void Update() {
        if (waitingForGameStart) {
            if (Input.GetButtonDown("Jump")) {
                NewGame();
            }
        } else if (waitingForGameEnd) {
        } else {
            if (Input.GetKey(KeyCode.Escape) && activePart) {
                AbandonPart();
            }
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            EndGame();
        }
    }

    public void SetActivePart(PartBehaviour partBehaviour) {
        activePart = partBehaviour;
    }

    public void AbandonPart() {
        if (activePart) {
            activePart.Die();
        }
        GoToPickState();
    }

    public void GoToWaitState() {
        partPickerManager.enabled = false;
        cameraBehaviour.target = null;
        waitingForGameEnd = false;
        waitingForGameStart = true;
    }

    public void GoToPickState() {
        activePart = null;
        partPickerManager.enabled = true;
        cameraBehaviour.target = null;
    }

    public void EndGame() {
        AbandonPart();
        podSpawner.DeployPod();
        waitingForGameEnd = true;
        partPickerManager.enabled = false;
    }

    public void NewGame() {
        timer.ResetTime();
        podSpawner.SpawnPod();
        activePod = podSpawner.currentPod;
        timer.enabled = true;
        partPickerManager.enabled = true;
        waitingForGameStart = false;
    }
}
