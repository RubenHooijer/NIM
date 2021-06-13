using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Oasez.Extensions.List {

    public static class ListExtensions {

        /// <summary>
        /// Shuffles the list randomly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list) {

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;

            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Returns a random element in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this IList<T> list) {
            System.Random random = new System.Random();
            return list[random.Next(0, list.Count - 1)];
        }

        /// <summary>
        /// Returns next element in list (looping)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetNext<T>(this IList<T> list, int index) {
            int nextTurnIndex = index + 1;
            if (nextTurnIndex >= list.Count) {
                nextTurnIndex = 0;
            }
            return list[nextTurnIndex];
        }

        /// <summary>
        /// Returns next element in list (looping)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T GetNext<T>(this IList<T> list, T item) => GetNext(list, list.IndexOf(item));

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TCastTo"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key of the value to get</param>
        /// <param name="value">The casted value of the specified key; Otherwise default value</param>
        /// <returns>True if the Dictionary<TKey, TValue> contains an element with specified key; otherwise, false</returns>
        public static bool TryGetCastedValue<TKey, TValue, TCastTo>(this Dictionary<TKey, TValue> dictionary, TKey key, out TCastTo value) where TCastTo : TValue {
            if (dictionary.TryGetValue(key, out TValue tmp)) {
                value = (TCastTo)tmp;
                return true;
            } else {
                value = default;
                return false;
            }
        }

    }

}