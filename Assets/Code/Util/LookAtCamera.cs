using UnityEngine;
using System.Collections;
namespace engine {
    public class LookAtCamera : MonoBehaviour {
        void Update() {
            transform.LookAt(Camera.main.transform);
        }
    }
}

