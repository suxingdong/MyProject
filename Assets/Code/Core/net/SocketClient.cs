using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
namespace engine {
    public class SocketClient {

        private Socket socket;
        private string host;
        private int port;
        private SocketListener listener;
        private IPEndPoint address;
        private volatile bool connected;
        private Thread readThread;
        private byte[] lengthBytes;

        private bool shuttingDown;

        private AsyncCallback writeCallback;

        //shut down application
        public void dispose() {
            shuttingDown = true;
            close();
        }

        public void init(string host, int port, SocketListener listener) {
            this.host = host;
            this.port = port;
            this.listener = listener;

            writeCallback = new AsyncCallback(onSent);

            lengthBytes = new byte[4];

            connect();
        }
        private void connect() {
            if (connected) return;
            try {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                address = new IPEndPoint(IPAddress.Parse(host), port);
                socket.BeginConnect(address, new System.AsyncCallback(onConnectionResult), socket);
            } catch (Exception error) {
                close();
                listener.onConnectionError(error);
            }

        }
        private void onConnectionResult(IAsyncResult result) {
            try {
                socket.EndConnect(result);
                readThread = new Thread(new ThreadStart(doRead));
                readThread.Start();
                connected = true;

            } catch (Exception error) {
                close();
                listener.onConnectionError(error);
            }
        }
        private void doRead() {
            while (!shuttingDown && connected) {
                try {
                    socket.Receive(lengthBytes);
                    byte[] data = new byte[ByteBuffer.toInt(lengthBytes, 0)];
                    socket.Receive(data);
                    listener.onData(data);
                } catch (Exception ex) {
                    close();
                    listener.onReadWriteError(ex);
                }
            }
        }
        public void send(ByteBuffer buffer) {
            if (!connected) return;
            try {
                buffer.beforeSend();
                socket.BeginSend(buffer.data, 0, buffer.writeIndex, SocketFlags.None, writeCallback, socket);
            } catch (Exception ex) {
                close();
                listener.onReadWriteError(ex);
            } finally {
                ByteBuffer.freeWrite(buffer);
            }
        }
        private void onSent(IAsyncResult result) {
            try {
                socket.EndSend(result);
            } catch (Exception ex) {
                close();
                listener.onReadWriteError(ex);
            }
        }
        private void close() {
            try {
                connected = false;
                socket.Close();
            } catch (Exception) {
            }

        }
        //disconnect manually
        public void disconnect() {
            close();
        }
    }

}
