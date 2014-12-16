using UnityEngine;
using System.Collections;
namespace engine {
    public class CDButton  {
        public string name;
        public GameObject go;
        public bool completed = true;

        protected GameObject mask;
        protected UIButton uiButton;
        protected UISprite cdSprite;

        protected float cdDuration=1;
        protected float speed=0.1f;


        public void init(GameObject go) {
            this.go = go;
            this.mask = go.getChild("mask");
            this.uiButton = go.GetComponent<UIButton>();
            this.cdSprite = mask.GetComponent<UISprite>();

            this.mask.SetActive(false);
            cdSprite.fillAmount = 0;
        }
        public void init2(GameObject go,UISprite _cdSprite)
        {
            this.go = go;
            this.mask = go.getChild("mask");
            this.cdSprite = _cdSprite;

            this.mask.SetActive(false);
            cdSprite.fillAmount = 0;
        }

       
        public void startCD() {
            if (uiButton != null) uiButton.enabled = false;
            this.completed = false;
            this.mask.SetActive(true);
            cdSprite.fillAmount = 1;
        }
        public void update() {
            if (completed) return;
            cdSprite.fillAmount -= speed * Time.deltaTime;
            if (cdSprite.fillAmount <=0) {
                cdSprite.fillAmount = 0;
                completed = true;
                onCompleted();
            }
        }
        public void onCompleted() {
            if (uiButton != null) uiButton.enabled = true;
            mask.SetActive(false);
            onCdFinish();
        }

        public virtual void onCdFinish()
        {

        }
    }
}

