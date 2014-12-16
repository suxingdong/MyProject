using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace engine {
    //value object pool.
    //thread unsafe,use it in a single thread.
    public class Pool<T> where T : new() {

        private Stack<T> items = new Stack<T>();
        private bool checkFreeDuplicate;//if check duplicate object in pool when freeing.

        public Pool() { }
        public Pool(bool checkFreeDuplicate) {
            this.checkFreeDuplicate = checkFreeDuplicate;
        }
        public T get() {
            if (items.Count == 0) return new T();
            return items.Pop();
        }
        public void free(T item) {
            if (checkFreeDuplicate) {
                if (items.Contains(item)) return;
            }
            items.Push(item);

        }
    }

}
