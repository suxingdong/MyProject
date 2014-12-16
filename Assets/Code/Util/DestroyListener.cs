using UnityEngine;
using System.Collections;
namespace engine {
    public class DestroyListener : MonoBehaviour {
        public delegate void Callback(object data);
        public Callback callback;
        public object data;
        public void reset(object data, Callback callback) {
            this.data = data;
            this.callback = callback;
        }
        void OnDestroy() {
            callback(data);
        }
    }

}
