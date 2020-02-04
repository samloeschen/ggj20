using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip titleScreenAudio;
    public AudioClip goToGameStinger;
    public AudioClip gameplayMusic;

    public AudioClip selectSound;
    public AudioClip attachSound;
    public AudioClip toggleSound;
    public AudioClip[] rewardShouts;

    public GameObject tempSourcePrefab;

    TempAudioSource mainMenuLoopSource;

    public void StopMainMenuMusic() {
        if (mainMenuLoopSource != null && mainMenuLoopSource.gameObject.activeSelf) {
            mainMenuLoopSource.Stop();
        }
    }

    public void PlayMainMenuMusic(float delay = 0f) {
        mainMenuLoopSource = PlayClip(titleScreenAudio, delay, loop: true);
    }

    public void PlayGameStinger(float delay = 0f) {
        PlayClip(goToGameStinger, delay);
    }

    public void PlayGameplayMusic(float delay = 0f) {
        PlayClip(gameplayMusic, delay);
    }

    public void PlayAttachSound(float delay = 0f) {
        PlayClip(attachSound, delay);
    }

    public void PlayToggleSound(float delay = 0f) {
        PlayClip(toggleSound, delay);
    }

    public void PlaySelectSound(float delay = 0f) {
        PlayClip(selectSound, delay);
    }

    public void PlayShout(float delay = 0f) {
        var idx = Random.Range(0, rewardShouts.Length - 1);
        var clip = rewardShouts[idx];
        rewardShouts[idx] = rewardShouts[rewardShouts.Length - 1];
        rewardShouts[rewardShouts.Length - 1] = clip;
        PlayClip(clip, delay);
    }

    public TempAudioSource PlayClip (AudioClip clip, float delay = 0f, bool loop = false) {
        var clone = PoolManager.PoolInstantiate(tempSourcePrefab, Vector3.zero, Quaternion.identity);
        if (clone.TryGetComponent<TempAudioSource>(out var source)) {
            source.PlayClip(clip, delay, loop);
            return source;
        }
        return null;
    }
}
