using UnityEngine;
using System.Collections;
using System;
namespace engine {
    public interface SocketListener {
        void onConnectionError(Exception error);
        void onReadWriteError(Exception error);

        void onData(byte[] data);
    }

}
