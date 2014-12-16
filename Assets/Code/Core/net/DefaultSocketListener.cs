using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace engine {
    public class DefaultSocketListener : SocketListener {
        private object[] producerQueue;

        private int length;
        private int usedIndex;
        private SocketDataConsumer consumer;
        public DefaultSocketListener(SocketDataConsumer consumer) {
            this.consumer = consumer;
            length = 100;
            producerQueue = new object[length];

        }
        public void onConnected() {
            Debug.Log("client connected");
        }

        public void onConnectionError(Exception error) {
            Debug.Log(error);
        }
        public void onReadWriteError(Exception error) {
            Debug.Log(error);
        }
        public void onData(byte[] data) {
            lock (producerQueue) {
                if (usedIndex >= length) {
                    object[] temp = new object[length + 10];
                    Array.Copy(producerQueue, 0, temp, 0, length);
                    producerQueue = temp;
                    length += 10;
                }
                producerQueue[usedIndex++] = data;
            }

        }
        public void consume() {
            lock (producerQueue) {
                if (usedIndex == 0) return;
                if (consumer._length < length) {
                    object[] temp = new object[length];
                    Array.Copy(producerQueue, 0, temp, 0, length);
                    consumer.data = temp;
                    consumer._length = length;
                } else {
                    Array.Copy(producerQueue, 0, consumer.data, 0, usedIndex);
                }

                consumer.size = usedIndex;
                usedIndex = 0;
            }
        }
    }

}
