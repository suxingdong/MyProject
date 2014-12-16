using UnityEngine;
using System.Collections;
namespace engine {
    public class Click : MonoBehaviour {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public delegate void VoidDelegatePress(GameObject go, bool isPressed);
        public VoidDelegatePress onPress;
        void OnClick() {
            if (onClick != null) {
                onClick(gameObject);
            }
        }
        void OnPress (bool isPressed){
            if (onPress != null)
            {
                onPress(gameObject, isPressed);
            }
        }
        public static void set(GameObject button, VoidDelegate onClick) {
            get(button).onClick = onClick;
        }
        public static Click get(GameObject go) {
            return go.addOnce<Click>();
        }

    }
}

