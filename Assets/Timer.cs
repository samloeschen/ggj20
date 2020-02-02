using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    public int startTime;
    public int currentTime;

    public TextMeshPro textMeshPro;

    float _t;

    public void ResetTime() {
        currentTime = startTime;
        textMeshPro?.SetText(currentTime.ToString());
    }

    void Update() {
        _t += Time.deltaTime;
        if (_t >= 1f) {
            currentTime--;
            textMeshPro?.SetText(currentTime.ToString());
            _t -= 1f;
            if (currentTime == 0) {
                GameManager.instance.EndGame();
            }
        }
    }
}





