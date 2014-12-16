using UnityEngine;
using System.Collections;
namespace engine {
    public interface IProgressListener {

        /// <summary>
        /// called by DownloadManager
        /// </summary>
        /// <param name="totalCount">how many bundles need to  check cache or download</param>
        /// <param name="completedCount">how many bundles have finished checking cache or downloading</param>
        /// <param name="currentBundleProgress">the progress of the currently been downloading bundle</param>
        void onProgress(int totalCount, int completedCount, float currentBundleProgress);
        void onProgress(float currentProgress, float currentBundleProgress);
        void onComplete();

    }

}
