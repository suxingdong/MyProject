using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace engine {
    //used only for Resources folder objects

    public class LocalResourceManager {
        //loaded prefabs
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
        //all cloned prefab instances
        private Dictionary<string, Stack<GameObject>> all = new Dictionary<string, Stack<GameObject>>();
        //remember cloned instances come from which prefab
        private Dictionary<GameObject, string> instances = new Dictionary<GameObject, string>();


        public GameObject get(string path) {
            GameObject prefab =null;
            if (prefabs.ContainsKey(path)) {
                prefab = prefabs[path];
            } else {
                prefab = Resources.Load<GameObject>(path);
                prefabs[path] = prefab;
            }
          
            GameObject instance;
            Stack<GameObject> stack;
            if (all.ContainsKey(path)) {
                stack = all[path];
                if (stack.Count > 0) {
                    instance = stack.Pop();
                    instances[instance] = path;
                    return instance;
                }
            }
            instance = GameObject.Instantiate(prefab) as GameObject;
            instances[instance] = path;
            return instance;
        }
        public void free(GameObject instance) {
            if (instances.ContainsKey(instance)) {
                string path = instances[instance];
                if (all.ContainsKey(path)) {
                    all[path].Push(instance);
                } else {
                    Stack<GameObject> stack = new Stack<GameObject>();
                    all[path] = stack;
                    stack.Push(instance);
                }
            } else {
                GameObject.Destroy(instance);
            }
        }
    }

}

