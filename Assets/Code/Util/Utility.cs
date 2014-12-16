using System;
using UnityEngine;

namespace engine {
    public class Utility {
        public const float PRECISION = 0.00001f;
        public static float toFloat(string value) {
            if (string.IsNullOrEmpty(value)) return 0;
            return float.Parse(value);
        }
        public static bool judgeFloatZero(float value) {
            if (Math.Abs(value) <= PRECISION) {
                return true;
            }
            return false;
        }
        public static float[] toFloatArray(string value, char split=',') {
            if (string.IsNullOrEmpty(value)) return null;
            string[] parts = value.Split(split);
            if (parts == null || parts.Length < 1) return null;
            int length = parts.Length;
            float[] result = new float[length];
            for (int i = 0; i < length; i++) {
                result[i] = toFloat(parts[i]);
            }
            return result;
        }
        public static int toInt(string value) {
            if (string.IsNullOrEmpty(value)) return 0;
            return int.Parse(value);
        }
        public static int[] toIntArray(string value, char split=',') {
            if (string.IsNullOrEmpty(value)) return null;
            string[] parts = value.Split(split);
            if (parts == null || parts.Length < 1) return null;
            int length = parts.Length;
            int[] result = new int[length];
            for (int i = 0; i < length; i++) {
                result[i] = toInt(parts[i]);
            }
            return result;
        }
        public static Vector3 toVector3(string value, char split = ',') {
            Vector3 vec = Vector3.zero;
            float[] splits = toFloatArray(value, split);
            if (splits != null) {
                if (splits.Length == 3)
                    vec = new Vector3(splits[0], splits[1], splits[2]);
                else if (splits.Length == 2)
                    vec = new Vector3(splits[0], 0, splits[1]);
            }
            return vec;
        }
        public static string[] toArray(string value, char split = ',') {
            string[] parts = value.Split(split);
            return parts;
        }
        //add a new element to an array.
        //the original array or the newly created array will be returned.
        public static T[] add<T>(T[] array, T o) {
            if (array == null) {
                array = new T[1];
                array[0] = o;
                return array;
            } else {
                T[] copy = new T[array.Length + 1];
                System.Array.Copy(array, 0, copy, 0, array.Length);
                copy[copy.Length - 1] = o;
                return copy;
            }
        }

        public static bool calSuccess(float rate, int baseRate = 100) {
            if (rate <= 0)
                return false;
            if (rate >= 100)
                return true;
            return rate > UnityEngine.Random.Range(0, 100);
        }

        public static Color valueOfColor(String hex)
        {
            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);
            int a = hex.Length != 8 ? 255 : Convert.ToInt32(hex.Substring(6, 2), 16);
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }

}
