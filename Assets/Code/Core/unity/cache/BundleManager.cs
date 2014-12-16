using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace engine {
    //represents an asset bundle
    public class Bundle {
        public string name;
        public bool completed;
        public bool started;
        public int version;
        public string error;//the download error if any
        public WWW www;
        public Dictionary<string, Object> prefabs;//initialized when needed.

        public AssetBundle sceneBundle;//used for scene asset bundle.

        public Bundle() { }
        public Bundle(string path) {
            this.name = path;
        }
        public void dispose() {
            if (sceneBundle != null) {
                sceneBundle.Unload(false);
                sceneBundle = null;
            }
            if (www != null) {
                www.Dispose();
                www = null;
            }
            if (prefabs != null) {
                foreach (GameObject go in prefabs.Values) {
                    GameObject.Destroy(go);
                }
                prefabs = null;
            }
        }
    }

    public class BundleManager : MonoBehaviour {
        public delegate void OnBundleLoaded();
        public delegate void OnSceneLoaded();
        private OnBundleLoaded onBundleLoaded;
        private OnSceneLoaded onSceneLoaded;


        public string baseURL;//download base url
        public Dictionary<string, int> versions = new Dictionary<string, int>();

        private Dictionary<string, Bundle> downloadedBundles = new Dictionary<string, Bundle>();
        private List<Bundle> pendingBundles;
        private bool downloading, completed;
        private Bundle currentBundle;//current downloading bundle
        private IProgressListener progressListener;
        private int totalCount, completedCount;

        private Dictionary<string, Bundle> loadedBundles = new Dictionary<string, Bundle>();
        private List<Bundle> loadingBundles = new List<Bundle>();
        private Dictionary<string, List<OnBundleLoaded>> pendings = new Dictionary<string, List<OnBundleLoaded>>();
        private bool checkPending,needNotifyPending;

        private bool loading;//if we're loading any bundle into memory
        private bool loadingScene;//if we're loading a level


        void Awake() {
            this.enabled = false;
            
        }
        public void disposeAllLoadedBundles() {
            foreach (Bundle bundle in loadedBundles.Values) {
                bundle.dispose();
            }
            loadedBundles.Clear();
        }
       
        //because www is asynchronous loading,client code have to call this before calling creating anything.
        private void loadPending() {
            foreach (string bundleName in pendings.Keys) {
                Bundle bundle = downloadedBundles[bundleName];
                if (!loadedBundles.ContainsKey(bundleName)) {
                    string url = baseURL + bundle.name;
                    bundle.www = WWW.LoadFromCacheOrDownload(url, bundle.version);
                    loadingBundles.Add(bundle);
                }
            }
            this.loading = loadingBundles.Count > 0;
            if (this.loading) {
                this.enabled = true;
                needNotifyPending = true;
            } else {
               notifyPending();
            }
        }
        private void notifyPending() {
            needNotifyPending = false;
            List<OnBundleLoaded> copy = new List<OnBundleLoaded>();
            foreach (List<OnBundleLoaded> listeners in pendings.Values) {
                foreach (OnBundleLoaded listener in listeners) {
                    copy.Add(listener);
                }
            }
            pendings.Clear();
            foreach (OnBundleLoaded listener in copy) {
                listener();
            }
        }
        //dynamically load a bundle.
        public void load(string bundleName, OnBundleLoaded onComplete) {
            if (loadedBundles.ContainsKey(bundleName)) {
                onComplete();
            } else {
                if (loading) {
                    if (pendings.ContainsKey(bundleName)) {
                        pendings[bundleName].Add(onComplete);
                    } else {
                        List<OnBundleLoaded> listener = new List<OnBundleLoaded>();
                        pendings[bundleName] = listener;
                        listener.Add(onComplete);
                    }
                    checkPending = true;
                } else {
                    bulkLoad(new string[] { bundleName }, onComplete);
                }
            }
        }
        //load multi bundles,this should be called when load scene.
        public void bulkLoad(string[] bundleNames,OnBundleLoaded onComplete) {
            loadingBundles.Clear();
            for (int i = bundleNames.Length - 1; i > -1; i--) {
                string bundleName = bundleNames[i];
                Bundle bundle = downloadedBundles[bundleName];
                if (!loadedBundles.ContainsKey(bundleName)) {
                    string url = baseURL + bundle.name;
                    bundle.www = WWW.LoadFromCacheOrDownload(url, bundle.version);
                    loadingBundles.Add(bundle);
                }
            }
            this.loading = loadingBundles.Count > 0;
            if (this.loading) {
                this.enabled = true;
                this.onBundleLoaded = onComplete;
            } else {
                onComplete();
            }
        }
        public void disposePrevSceneBundle(string name) {
            if (string.IsNullOrEmpty(name)) return;
            if (downloadedBundles.ContainsKey(name))
            {
                Bundle bundle = downloadedBundles[name];
                if (bundle == null) return;
                bundle.dispose();
            }
        }
        public void loadScene(string bundleName, OnSceneLoaded onComplete) {
            Bundle bundle = downloadedBundles[bundleName];
            string url = baseURL + bundle.name;
            bundle.www = WWW.LoadFromCacheOrDownload(url, bundle.version);

            loadingBundles.Clear();
            loadingBundles.Add(bundle);
            this.enabled = true;
            this.loading = true;
            this.loadingScene = true;
            this.onSceneLoaded = onComplete;
        }
        
        //create an new game object from a loaded asset bundle.
        public GameObject create(string bundleName, string assetName) {
            Bundle bundle = downloadedBundles[bundleName];
            if (!loadedBundles.ContainsKey(bundleName)) {
                throw new System.Exception(bundleName + " not loaded,call load() first.");
            }
            if (bundle.prefabs == null) bundle.prefabs = new Dictionary<string, Object>();
            Object prefab = null;
            if (bundle.prefabs.ContainsKey(assetName)) {
                prefab = bundle.prefabs[assetName];
            } else {
                prefab = bundle.www.assetBundle.Load(assetName, typeof(GameObject));
                if (prefab == null) {
                    throw new System.Exception(bundleName + "/"+assetName+" not found. maybe you need clear cache.");
                }
                bundle.prefabs[assetName] = prefab;
            }
            return GameObject.Instantiate(prefab) as GameObject;
           
        }

        public void downlod(List<Bundle> bundles, IProgressListener progressListener = null) {
            this.pendingBundles = bundles;
            this.progressListener = progressListener;
            this.completed = false;
            this.downloading = false;
            this.currentBundle = null;

            for (int i = pendingBundles.Count - 1; i > -1; i--) {
                Bundle bundle = pendingBundles[i];
                if (downloadedBundles.ContainsKey(bundle.name)) {
                    pendingBundles.RemoveAt(i);
                } else {
                    bundle.completed = false;
                    bundle.started = false;
                    bundle.error = null;
                    bundle.version = versions[bundle.name];
                }
            }

            totalCount = pendingBundles.Count;
            completedCount = 0;
            if (totalCount > 0) this.enabled = true;
        }
        public void Update() {
            if (loading) {
                //loading into memory process
                for (int i = loadingBundles.Count - 1; i > -1; i--) {
                    Bundle bundle = loadingBundles[i];
                    if (bundle.www.isDone) {
                        loadedBundles[bundle.name] = bundle;
                        loadingBundles.RemoveAt(i);
                        if (loadingScene) {
                            bundle.sceneBundle = bundle.www.assetBundle;
                        }
                    }
                }
                loading = loadingBundles.Count > 0;
                if (!loading) {
                    this.enabled = checkPending;
                    if (this.loadingScene) {
                        this.loadingScene = false;
                        OnSceneLoaded callback = this.onSceneLoaded;
                        this.onSceneLoaded = null;
                        if (callback != null) callback();
                    } else {
                        OnBundleLoaded callback = this.onBundleLoaded;
                        this.onBundleLoaded = null;
                        if (callback != null) callback();
                    }
                    if (needNotifyPending) {
                        notifyPending();
                    }
                }
            } else if (checkPending) {
                checkPending = false;
                loadPending();
            } else {
                //downloading process
                if (pendingBundles == null) return;
                if (!downloading) {
                    for (int i = pendingBundles.Count - 1; i > -1; i--) {
                        Bundle bundle = pendingBundles[i];
                        if (bundle.completed) {
                            pendingBundles.RemoveAt(i);
                            completedCount++;
                        } else if (!bundle.started) {
                            currentBundle = bundle;
                            currentBundle.started = true;
                            downloading = true;
                            StartCoroutine(downloadBundle(currentBundle));
                            break;
                        }
                    }
                }

                completed = pendingBundles.Count == 0;
                //notify progress listener if any
                if (progressListener != null) {
                    //current single bundle download progress
                    float currentBundleProgress = 0;
                    if (currentBundle != null && currentBundle.www != null) currentBundleProgress = currentBundle.www.progress;
                    else if (completed) {
                        currentBundleProgress = 1f;
                        totalCount = completedCount;
                    }
                    progressListener.onProgress(totalCount, completedCount, currentBundleProgress);
                    if (completed) {
                        IProgressListener listener = progressListener;
                        progressListener = null;
                        StartCoroutine(notifyCompleted(listener));
                    }
                }
            }
        }
        private IEnumerator notifyCompleted(IProgressListener listener) {
            yield return 2;
            this.enabled = false;
            listener.onComplete();
        }
        private IEnumerator downloadBundle(Bundle bundle) {
            while (!Caching.ready)
                yield return null;
            string url = baseURL + bundle.name;

            WWW www = bundle.www = WWW.LoadFromCacheOrDownload(url, bundle.version);
            yield return www;
            if (www.error != null) {
                bundle.error = www.error;
                Debug.Log(www.error);
            }
            downloadedBundles[bundle.name] = bundle;
            bundle.completed = true;
            bundle.www.assetBundle.Unload(true);
            bundle.www.Dispose();
            bundle.www = null;
            www.Dispose();
            downloading = false;

        }
    }

}
