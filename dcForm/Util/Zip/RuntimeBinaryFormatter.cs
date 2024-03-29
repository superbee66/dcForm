﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using dCForm.Storage.Sql;
using Newtonsoft.Json.Serialization;

namespace dCForm.Util.Zip
{
    /// <summary>
    ///     Binary operations with custom SerializationBinder to look all over the place to find the right dlls if needed. This
    ///     was moved out of the utility section as it meets specific needs to this solution that does many runtime assembly
    ///     loading & changing namespaces over time.
    /// </summary>
    public static class RuntimeBinaryFormatter
    {
        public static readonly BinaryFormatter
            Formatter = new BinaryFormatter
            {
            },
            CloneFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone))
            {
            };

        public static T Clone<T>(this T o)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                CloneFormatter.Serialize(memoryStream, o);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T) CloneFormatter.Deserialize(memoryStream);
            }
        }

        public static T FromBytes<T>(this byte[] b)
        {
            using (MemoryStream memoryStream = new MemoryStream(b))
                return (T) Formatter.Deserialize(memoryStream);
        }

        public static byte[] ToBytes(this object o)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //TODO:have a closer look at this as this is questionable
                Formatter.Serialize(memoryStream, o);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }


        private static string ReplaceOldNames(string typeBindingExpression)
        {
            return typeBindingExpression
                //.Replace("dCForm.LightDoc, dCForm.Client", "dCForm.LightDoc, dCForm")
                //.Replace("dCForm.Client, ", "dCForm, ")
                ;
        }

        private static readonly Dictionary<string, Type> _BinaryDeserializationTypeDictionary = new Dictionary<string, Type>();
    }
}