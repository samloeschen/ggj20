using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class PoolManager : MonoBehaviour {

    public static PoolManager Instance;
    public FastList<DelayedDestroyData> delayedDestroyDataBuffer;
    public FastList<DelayedInstantiateData> delayedInstantiateDataBuffer;

    public struct DelayedDestroyData {
        public float delay;
        public float timestamp;
        public GameObject target;
        public bool removeParent;
        public Action callback;
    }

    public struct DelayedInstantiateData {
        public Vector3 position;
        public Quaternion rotation;
        public float delay;
        public float timestamp;
        public GameObject prefab;
        public Action<GameObject> callback;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SceneHook() {
        var managerObject = new GameObject("PoolManager");
        DontDestroyOnLoad(managerObject);
        Instance = managerObject.AddComponent<PoolManager>();
        Instance.Allocate();
        SceneManager.sceneUnloaded += (Scene s) => {
            pools.Clear();
        };
    }

    void Allocate() {
        delayedDestroyDataBuffer = new FastList<DelayedDestroyData>(1024);
        delayedInstantiateDataBuffer = new FastList<DelayedInstantiateData>(1024);
    }

    public void Update() {
        DelayedInstantiateData instantiateData;
        for (int i = 0; i < delayedInstantiateDataBuffer.count; i++) {
            instantiateData = delayedInstantiateDataBuffer[i];
            if (Time.time - instantiateData.timestamp > instantiateData.delay) {
                var obj = PoolInstantiate(instantiateData.prefab, instantiateData.position, instantiateData.rotation);
                delayedInstantiateDataBuffer.RemoveFast(i);
                if (instantiateData.callback != null) {
                    instantiateData.callback(obj);
                }
            }
        }
        DelayedDestroyData destroyData;
        for (int i = 0; i < delayedDestroyDataBuffer.count; i++) {
            destroyData = delayedDestroyDataBuffer[i];
            if (Time.time - destroyData.timestamp > destroyData.delay) {
                PoolDestroy(destroyData.target, removeParent: destroyData.removeParent);
                delayedDestroyDataBuffer.RemoveFast(i);
                if (destroyData.callback != null) {
                    destroyData.callback();
                }
            }
        }
    }

    public static Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>(1024);
    /*
	* Use this function for general purpose GameObject instantiation. It will instantiate the
	* a pooled instance immediately. If it doesn't find a pooled instance, it uses GetInstanceInactive()
	* to make a new one, and immediately instantiates and activates that. If the instance matches one already
	* in the pool (for example, one obtained from GetInstanceInactive), it just instantiates it.
	*/
    public static GameObject PoolInstantiate(GameObject prefab) {
        if (prefab.TryGetComponent<Transform>(out var transform)) {
            return PoolInstantiate(prefab, transform.position, transform.rotation);
        }
        return null;
    }

    public static void PoolInstantiateLater(GameObject prefab, Vector3 position, Quaternion rotation, float delay, Action<GameObject> callback = null) {
        var data = new DelayedInstantiateData {
            position  = position,
            rotation  = rotation,
            delay     = delay,
            timestamp = Time.time,
            prefab    = prefab,
            callback  = callback,
        };
        Instance.delayedInstantiateDataBuffer.Add(data);
    }


    public static GameObject PoolInstantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
        if(prefab == null) return null;
        GameObject tempObject = null;
        PoolPrefabTracker tracker = null;
        bool makeNew = false;
        if (pools.ContainsKey(prefab)) {
            if (pools[prefab].Count > 0) {
                //pool exists and has unused instances
                tempObject = pools[prefab][0];
                pools[prefab].RemoveAt(0);
                tempObject.transform.position = position;
                tempObject.transform.rotation = rotation;
                tempObject.transform.localScale = prefab.transform.localScale;
                tracker = tempObject.GetComponent<PoolPrefabTracker>();
                tracker.SetReleased();
                tempObject.SetActive(true);
                return tempObject;
            } else {
                //pool exists but is empty
                makeNew = true;
            }
        } else {
            //pool for this prefab does not yet exist
            pools.Add(prefab, new List<GameObject>(1024));
            makeNew = true;
        }
        if (makeNew) {
            tempObject = GameObject.Instantiate(prefab, position, rotation);
            tracker = tempObject.AddComponent<PoolPrefabTracker>();
            tracker.prefab = prefab;
            tracker.SetReleased();
            return tempObject;
        }
        return tempObject;
    }
    static public GameObject PoolInstantiate (MonoBehaviour mb, Vector3 position, Quaternion rotation){
        return PoolInstantiate(mb.gameObject, position, rotation);
    }
    static public void PoolDestroy (GameObject target, bool removeParent = false) {
        if (!target) return;

        if (removeParent) {
            target.transform.SetParent(null);
        }

        PoolPrefabTracker tracker = target.GetComponent<PoolPrefabTracker>();
        if (tracker) {
            if (tracker.pooled) { return; }
            GameObject prefab = tracker.prefab;
            tracker.SetPooled();
            target.SetActive(false);
            if (!pools.ContainsKey(prefab)) return;
            pools[prefab].Add(target);
        } else {
            tracker = target.AddComponent<PoolPrefabTracker>();
            tracker.prefab = target;
            if (!pools.ContainsKey(target)) {
                pools.Add(target, new List<GameObject>());
            }
            PoolDestroy(target);
        }
    }

    public static void PoolDestroyLater(GameObject target, float delay, bool removeParent = false, Action callback = null) {
        var data = new DelayedDestroyData {
            target       = target,
            delay        = delay,
            timestamp    = Time.time,
            removeParent = removeParent,
            callback     = callback
        };
        Instance.delayedDestroyDataBuffer.Add(data);
    }
}
public class PoolPrefabTracker : MonoBehaviour {
    public delegate void PoolEventHandler();
    public event Action OnPooled; //removed from pool
    public event Action OnReleased; //returned to pool
    public GameObject prefab;
    public bool pooled;

    public void SetPooled(){
        pooled = true;
        if(OnPooled != null) OnPooled();
    }
    public void SetReleased(){
        pooled = false;
        if(OnReleased != null) OnReleased();
    }
}

