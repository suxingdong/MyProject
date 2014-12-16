using UnityEngine;
using System.Collections;
using System;
namespace engine {

    public class NetManager {

        private SocketClient socket;
        private DefaultSocketListener listener;
        private SocketDataConsumer consumer;
        private IMessageHandler handler;

        private ByteBuffer buffer;


        public NetManager(IMessageHandler handler) {
            this.handler = handler;
            consumer = new SocketDataConsumer();
            listener = new DefaultSocketListener(consumer);
            buffer = new ByteBuffer();
        }

        public void connect(string host, int port) {
            if (socket == null) socket = new SocketClient();
            socket.init(host, port, listener);
        }
        //disconnect manually
        public void disconnect() {
            socket.disconnect();
        }
        public void send(ByteBuffer buffer) {
            socket.send(buffer);
        }

        public void checkMessage() {
            listener.consume();
            int size = consumer.size;
            if (size == 0) return;
            object[] data = consumer.data;
            try {
                for (int i = 0; i < size; i++) {
                    buffer.reset((byte[])data[i]);
                    handler.handle(buffer);
                }
            } finally {
                consumer.size = 0;
            }
        }
    }
}
