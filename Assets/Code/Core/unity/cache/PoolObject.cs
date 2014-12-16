using UnityEngine;
using System.Collections;
namespace engine {
    //poolable objects like bullet etc.
    public class PoolObject {
        public string key;//cache key,usually bundleName_assetName
        public GameObject go;//created gameobject from bundle
        public Transform transform;//the game object's transform for easy access

        public bool completed;//if this object is completed and suitable for recycling

        public bool inPool;//used only for pool manager.
        
        //called when created from pool on first time use.
        public void setGameObject(GameObject go) {
            this.go = go;
            this.transform = go.transform;
            this.completed = false;
        }
        //public void setTransform(Vector3 position, Quaternion rotation) {
        //    transform.position = position;
        //    transform.rotation = rotation;
        //}
    }

}
