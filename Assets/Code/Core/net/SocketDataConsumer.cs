using UnityEngine;
using System.Collections;
namespace engine {
    //used in unity ui thread.
    //see NetManager.

    public class SocketDataConsumer {
        public object[] data;
        public int size;//actual size.

        internal int _length;

        public SocketDataConsumer() {
            _length = 100;
            data = new object[_length];
        }
    }

}
