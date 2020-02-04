using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAudioSource : MonoBehaviour {
    public AudioSource source;
    bool played = false;

    void OnEnable() {
        played = false;
    }

    public void PlayClip(AudioClip clip, float delay = 0f, bool loop = false) {
        if (source == null) {
            source = gameObject.AddComponent<AudioSource>();
        }
        source.clip = clip;
        source.loop = loop;
        source.Play();
        played = true;
    }

    public void Stop() {
        source.Stop();
        PoolManager.PoolDestroy(gameObject);
    }

    void Update() {
        if (source != null && !source.isPlaying && played && !source.loop) {
            PoolManager.PoolDestroy(gameObject);
        }
    }
}
