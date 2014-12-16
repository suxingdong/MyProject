using UnityEngine;
using System.Collections;
namespace engine {
    //itween update callback helper.
    public class TweenUpdate : MonoBehaviour {
        public delegate void OnUpdate();
        public OnUpdate onUpdate;

        public void update() {
            Debug.Log("4444");
        }

        public static void set(GameObject go, OnUpdate update) {
            go.addOnce<TweenUpdate>().onUpdate = update;
    }
    }
}

