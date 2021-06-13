using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroFormatter;

namespace Oasez.Networking {

    public static class TransportSerializer {

        public delegate byte[] SerializeMethod(object customObject);
        public delegate object DeserializeMethod(byte[] customData);

        private static Dictionary<Type, byte> typeToTypecode = new Dictionary<Type, byte>();
        private static Dictionary<byte, Type> typecodeToType = new Dictionary<byte, Type>();
        private static Dictionary<byte, (SerializeMethod SerializeMethod, DeserializeMethod DeserializeMethod)> typecodeToSerializationMethods = new Dictionary<byte, (SerializeMethod, DeserializeMethod)>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            RegisterType(typeof(string), 255, TypeSerializers.ZeroFormatterSerialize<string>, TypeSerializers.ZeroFormatterDeserialize<string>);
            RegisterType(typeof(int), 254, TypeSerializers.ZeroFormatterSerialize<int>, TypeSerializers.ZeroFormatterDeserialize<int>);
            RegisterType(typeof(byte), 253, TypeSerializers.ZeroFormatterSerialize<byte>, TypeSerializers.ZeroFormatterDeserialize<byte>);
            RegisterType(typeof(Array), 252, null, DeserializeCollection);
            RegisterType(typeof(bool), 251, TypeSerializers.ZeroFormatterSerialize<bool>, TypeSerializers.ZeroFormatterDeserialize<bool>);
        }

        public static void RegisterType(Type customType, byte typeCode, SerializeMethod serializeMethod, DeserializeMethod deserializeMethod) {
            typeToTypecode[customType] = typeCode;
            typecodeToType[typeCode] = customType;
            typecodeToSerializationMethods[typeCode] = (serializeMethod, deserializeMethod);
        }

        public static TransportObjectData Serialize(object customObject) {
            Type objectType = customObject.GetType();
            if (objectType.GetInterface(nameof(IEnumerable)) != null && objectType != typeof(string)) {
                return SerializeCollection(customObject);
            }

            //objectType = objectType.IsEnum ? objectType.GetEnumUnderlyingType() : objectType;
            if (!typeToTypecode.TryGetValue(objectType, out byte typeCode)) {
                Debug.LogError($"{objectType} not registered to serializer, please use TransportSerializer.RegisterType()");
                return null;
            }

            byte[] data = typecodeToSerializationMethods[typeCode].SerializeMethod(customObject);
            return new TransportObjectData(typeCode, data);
        }


        public static object Deserialize(TransportObjectData objectData) => Deserialize(objectData.Typecode, objectData.Data);
        public static object Deserialize(byte typecode, byte[] customData) {
            if (!typecodeToSerializationMethods.ContainsKey(typecode)) {
                Debug.LogError($"Received unregistered typecode {typecode} please use TransportSerializer.RegisterType()");
            }

            object customObject = typecodeToSerializationMethods[typecode].DeserializeMethod(customData);
            return customObject;
        }

        private static TransportObjectData SerializeCollection(object customObject) {
            Type collectionType = customObject.GetType();
            Type objectType = collectionType.IsArray ? collectionType.GetElementType() : collectionType.GetGenericArguments()[0];
            //objectType = objectType.IsEnum ? objectType.GetEnumUnderlyingType() : objectType;

            if (!typeToTypecode.TryGetValue(objectType, out byte typeCode)) {
                Debug.LogError($"{objectType} not registered to serializer, please use TransportSerializer.RegisterType()");
                return null;
            }
            IEnumerable collection = (IEnumerable)customObject;

            int index = 0;
            List<byte[]> dataList = new List<byte[]>();
            foreach (object item in collection) {
                dataList.Add(typecodeToSerializationMethods[typeCode].SerializeMethod(item));
                index++;
            }

            CollectionData collectionData = new CollectionData() {
                Typecode = typeCode,
                DataArray = dataList.ToArray(),
            };

            return new TransportObjectData(252, ZeroFormatterSerializer.Serialize(collectionData));
        }

        private static object DeserializeCollection(byte[] customData) {
            CollectionData collectionData = ZeroFormatterSerializer.Deserialize<CollectionData>(customData);
            Type itemType = typecodeToType[collectionData.Typecode];
            List<object> objectList = new List<object>();

            for (int i = 0; i < collectionData.DataArray.Length; i++) {
                Debug.Log($"Deserialized {collectionData.Typecode}, to type {Deserialize(collectionData.Typecode, collectionData.DataArray[i]).GetType()}");
                objectList.Add(Deserialize(collectionData.Typecode, collectionData.DataArray[i]));
            }
            
            Array array = Array.CreateInstance(itemType, collectionData.DataArray.Length);
            Array.Copy(objectList.ToArray(), array, objectList.Count);
            return array;
        }

    }

    public static class TypeSerializers {

        public static byte[] ZeroFormatterSerialize<T>(object customObject) {
            return ZeroFormatterSerializer.Serialize((T)customObject);
        }

        public static object ZeroFormatterDeserialize<T>(byte[] customData) {
            return ZeroFormatterSerializer.Deserialize<T>(customData);
        }

    }

}