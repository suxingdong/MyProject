using UnityEngine;
using System.Collections;
namespace engine {
    public static class Extensions {
        public static void resetLocalTransform(this GameObject go) {
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            //go.transform.localScale = Vector3.one;
        }
        public static GameObject getChild(this GameObject go, string path) {
            Transform t= go.transform.FindChild(path);
            if(t!=null) return t.gameObject;
            return null;
        }
        public static T getChildComponent<T>(this GameObject go, string path) where T : Component {
            GameObject result = go.getChild(path);
            if (result == null) return null;
            return result.GetComponent<T>();
        }
        public static T addOnce<T>(this GameObject g) where T : Component {
            T c = g.GetComponent<T>();
            if (c == null) {
                c = g.AddComponent<T>();
            }
            return c;
        }

        public static Transform T(this GameObject go, string path) {
            Transform t = go.transform.Find(path);
            if (t != null)
                return t;
            return null;
        }

        public static Vector3 T2V(this Transform t, float offset, int index) {
            Vector3 vec = Vector3.zero;
            for (int i = 0; i < 2; i++) {
                if (i == index)
                    vec[i] = t.localPosition[i] + offset;
                else {
                    vec[i] = t.localPosition[i];
                }
            }
            return vec;
        }

        public static void D<T>(this GameObject go) where T : Component {
            T t = go.GetComponent<T>();
            if(t != null)
                Object.Destroy(t);
        }

        ////////////////////NGUI extension//////////////////
        /// <summary>
        /// 
        /// </summary>
        /// <param name="go">the game object has a UILabel child component</param>
        /// <returns></returns>
        public static string getText(this GameObject go) {
            UILabel label = go.GetComponentInChildren<UILabel>();
            return label.text;
        }
        public static void setText(this GameObject go, string text) {
            UILabel label = go.GetComponentInChildren<UILabel>();
            label.text = text;
        }
        public static string getInput(this GameObject go) {
            UIInput input = go.GetComponentInChildren<UIInput>();
            return input.value;
        }
        public static void setInput(this GameObject go, string text) {
            UIInput label = go.GetComponentInChildren<UIInput>();
            label.value = text;
        }
       
    }

}
