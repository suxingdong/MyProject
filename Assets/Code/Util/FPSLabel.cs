using UnityEngine;
using System.Collections;
namespace engine {
    public class FPSLabel : MonoBehaviour {

        float deltaTime = 0.0f;
        private Rect rect;

        void Start() {
            rect = new Rect(0, 0, Screen.width, Screen.height * 0.5f);
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }
        GUIStyle style = new GUIStyle();
        void OnGUI() {
            int w = Screen.width, h = Screen.height;
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 80;
            style.normal.textColor = Color.white;// new Color(1, 1, 1, 1.0f);
            float msec = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}
