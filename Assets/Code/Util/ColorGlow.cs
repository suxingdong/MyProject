using UnityEngine;
using System.Collections;

namespace engine {
    public class ColorGlow : MonoBehaviour{
        public Color beginColor = new Color(0,0,0,0);
        public Color toColor = new Color(1, 1, 1, 1);
        public float duration = 1;
        public int maxCount = -1;


        private MeshRenderer[] renderers;
        private float deltaR, deltaG, deltaB, deltaA;
        private bool toUp = true;
        private Color color;
        private int count;
        private bool completed;
        private float playTime;
        void Awake()
        {
            renderers = GetComponents<MeshRenderer>();
            deltaR = (toColor.r - beginColor.r) / duration;
            deltaG = (toColor.r - beginColor.r) / duration;
            deltaB = (toColor.r - beginColor.r) / duration;
            deltaA = (toColor.r - beginColor.r) / duration;
            if (renderers != null){
                Shader shader = Shader.Find("echoLogin/Light/10-Fastest-Additive2");
                color = beginColor;
                foreach (MeshRenderer mr in renderers)
                {
                    for (int i = 0; i < mr.materials.Length; i++)
                    {
                        mr.materials[i].shader = shader;
                        mr.materials[i].SetColor("_TintColor", color);
                    }
                }
            } 
            toUp = true;
            count = 0;
            playTime = 0;
            completed = false;
        }
        void Update()
        {
            if (renderers == null || renderers.Length == 0)
                return;
            if (completed) return;
            
            if (toUp){
                 color.r += deltaR*Time.deltaTime;
                 color.g += deltaG*Time.deltaTime;
                 color.b += deltaB*Time.deltaTime;
                 color.a += deltaA*Time.deltaTime;
            }else {
                 color.r -= deltaR*Time.deltaTime;
                 color.g -= deltaG*Time.deltaTime;
                 color.b -= deltaB*Time.deltaTime;
                 color.a -= deltaA*Time.deltaTime;
            }
            foreach (MeshRenderer mr in renderers){
                for (int i = 0; i < mr.materials.Length; i++){
                     mr.materials[i].SetColor("_TintColor", color);
                }
            }
            if (playTime >= duration)
            {
                playTime = 0;
                toUp = !toUp;
                if (maxCount != -1)
                {
                    count++;
                    if (count >= maxCount) completed = true;
                }
            }
            playTime += Time.deltaTime;
        }
        
    }
}

