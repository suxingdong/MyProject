using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace engine {
    //downloaded bundle resource object pool.
    public class BundleObjectPool {
        public static BundleManager bundleManager;//assigned when app boots. all pools share the same bundle manager.

        public delegate object NewObject();
        public NewObject creator;
        public BundleObjectPool(NewObject creator) {
            this.creator = creator;
        }

        //key: bundleName_assetName
        private Dictionary<string, Stack> cache = new Dictionary<string, Stack>();

        public PoolObject get(string bundleName, string assetName) {
            PoolObject po;
            string key = bundleName + "_" + assetName;
            if (cache.ContainsKey(key)) {
                Stack pool = cache[key];
                if (pool.Count > 0) {
                    po = pool.Pop() as PoolObject;
                    po.setGameObject(po.go);
                } else {
                    po = creator() as PoolObject;
                    GameObject go = bundleManager.create(bundleName, assetName);
                    po.setGameObject(go);
                }
            } else {
                Stack pool = new Stack();
                cache[key] = pool;

                po = creator() as PoolObject;
                GameObject go = bundleManager.create(bundleName, assetName);
                po.setGameObject(go);
            }
            po.key = key;
            return po;

        }
        public void free(PoolObject item) {
            Stack pool = cache[item.key];
            pool.Push(item);
        }
    }

}
