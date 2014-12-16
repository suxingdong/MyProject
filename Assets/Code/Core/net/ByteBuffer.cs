using UnityEngine;
using System.Collections;
using System;
using System.Text;
namespace engine {
    public class ByteBuffer {
        public static Encoding UTF8 = Encoding.UTF8;
        public static int MAX = 256;//max length for sending out.

        //private static Pool<ByteBuffer> readPool = new Pool<ByteBuffer>();
        private static Pool<ByteBuffer> writePool = new Pool<ByteBuffer>();

        //since read operation always single thread,we share these temp.
        private static byte[] bytes8 = new byte[8];
        private static byte[] bytes4 = new byte[4];

        /// <summary>
        /// create a write buffer.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ByteBuffer get(int code) {
            ByteBuffer buffer;
            lock (writePool) {
                buffer = writePool.get();
            }
            if (buffer.data == null) buffer.data = new byte[MAX];
            buffer.readIndex = 0;
            buffer.writeIndex = 0;

            buffer.writeInt(0);
            buffer.writeInt(code);
            return buffer;

        }
        internal static void freeWrite(ByteBuffer data) {
            lock (writePool) {
                writePool.free(data);
            }
        }

        //public static ByteBuffer get(byte[] data) {
        //    ByteBuffer buffer;
        //    lock (readPool) {
        //        buffer = readPool.get();
        //    }

        //    buffer.data = data;
        //    buffer.readIndex = 0;
        //    buffer.writeIndex = 0;
        //    return buffer;
        //}

        //public static void freeRead(ByteBuffer[] byteArrays,int size) {
        //    lock (readPool) {
        //        for (int i = 0; i < size; i++) {
        //            readPool.free(byteArrays[i]);
        //        }
        //    }
        //}


        public static int toInt(byte[] data, int index) {
            return (data[index] & 0xff) << 24 |
                   (data[index + 1] & 0xff) << 16 |
                   (data[index + 2] & 0xff) << 8 |
                    data[index + 3] & 0xff;
        }

        internal int readIndex;
        internal int writeIndex;
        internal byte[] data;

        public void reset(byte[] data) {
            this.data = data;
            this.writeIndex = 0;
            this.readIndex = 0;
        }

        public void beforeSend() {
            int value = writeIndex - 4;
            data[0] = (byte)(value >> 24);
            data[1] = (byte)(value >> 16);
            data[2] = (byte)(value >> 8);
            data[3] = (byte)value;
        }
        //////////////////////reads//////////////////////////////////////
        public int readByte() {
            return data[readIndex++];
        }

        public int readShort() {
            int value = (short)(data[readIndex] << 8 | data[readIndex + 1] & 0xFF);
            readIndex += 2;
            return value;
        }
        public int readInt() {
            int value = (data[readIndex] & 0xff) << 24 |
                   (data[readIndex + 1] & 0xff) << 16 |
                   (data[readIndex + 2] & 0xff) << 8 |
                    data[readIndex + 3] & 0xff;
            readIndex += 4;
            return value;
        }
        public long readLong() {
            long value = ((long)data[readIndex] & 0xff) << 56 |
                    ((long)data[readIndex + 1] & 0xff) << 48 |
                    ((long)data[readIndex + 2] & 0xff) << 40 |
                    ((long)data[readIndex + 3] & 0xff) << 32 |
                    ((long)data[readIndex + 4] & 0xff) << 24 |
                    ((long)data[readIndex + 5] & 0xff) << 16 |
                    ((long)data[readIndex + 6] & 0xff) << 8 |
                     (long)data[readIndex + 7] & 0xff;
            readIndex += 8;
            return value;
        }
        public float readFloat() {
            float value;
            if (BitConverter.IsLittleEndian) {
                byte[] temp = bytes4;
                for (int i = 3; i >= 0; i--) {
                    temp[i] = data[readIndex++];
                }
                value = BitConverter.ToSingle(temp, 0);
            } else {
                value = BitConverter.ToSingle(data, readIndex);
                readIndex += 4;
            }

            return value;
        }

        public double readDouble() {
            double value;
            if (BitConverter.IsLittleEndian) {
                byte[] temp = bytes8;
                for (int i = 7; i >= 0; i--) {
                    temp[i] = data[readIndex++];
                }
                value = BitConverter.ToDouble(temp, 0);
            } else {
                value = BitConverter.ToDouble(data, readIndex);
                readIndex += 8;
            }
            return value;
        }
        public string readUTF() {
            int count = readShort();
            string value = UTF8.GetString(data, readIndex, count);
            readIndex += count;
            return value;
        }
        public byte[] readBytes(int length) {
            byte[] value = new byte[length];
            Buffer.BlockCopy(data, readIndex, value, 0, length);
            readIndex += length;
            return value;
        }


        //////////////////////writes//////////////////////////////////////

        public void writeByte(int value) {
            data[writeIndex++] = (byte)value;
        }
        public void writeShort(int value) {
            byte[] temp = BitConverter.GetBytes((short)value);
            if (BitConverter.IsLittleEndian) {
                data[writeIndex++] = temp[1];
                data[writeIndex++] = temp[0];
            } else {
                data[writeIndex++] = temp[0];
                data[writeIndex++] = temp[1];
            }

        }
        public void writeInt(int value) {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                for (int i = 3; i >= 0; i--) {
                    data[writeIndex++] = temp[i];
                }
            } else {
                Buffer.BlockCopy(temp, 0, data, writeIndex, 4);
                writeIndex += 4;
            }
        }
        public void writeFloat(float value) {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                for (int i = 3; i >= 0; i--) {
                    data[writeIndex++] = temp[i];
                }
            } else {
                Buffer.BlockCopy(temp, 0, data, writeIndex, 4);
                writeIndex += 4;
            }
        }
        public void writeLong(long value) {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                for (int i = 7; i >= 0; i--) {
                    data[writeIndex++] = temp[i];
                }
            } else {
                Buffer.BlockCopy(temp, 0, data, writeIndex, 8);
                writeIndex += 8;
            }

        }
        public void writeUTF(string value) {
            byte[] temp = UTF8.GetBytes(value);
            writeShort(temp.Length);
            Buffer.BlockCopy(temp, 0, data, writeIndex, temp.Length);
            writeIndex += temp.Length;
        }
        public void writeBytes(byte[] value) {
            Buffer.BlockCopy(value, 0, data, writeIndex, value.Length);
            writeIndex += value.Length;
        }
    }
}
