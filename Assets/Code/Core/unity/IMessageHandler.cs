using UnityEngine;
using System.Collections;
namespace engine {
    public interface IMessageHandler {
        //because the buffer is reused, the method must finish using it when exiting.
        void handle(ByteBuffer buffer);
    }

}
