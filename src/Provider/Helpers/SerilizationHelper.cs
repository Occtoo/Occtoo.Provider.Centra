using AutoMapper.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Occtoo.Provider.Centra.Helpers
{
    public class SerializationHelper
    {
        #region CreateAndInitInstance
        public static T CreateAndInitInstance<T>(JObject jsonObj)
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { });

            if (jsonObj != null)
            {
                var type = instance.GetType();
                foreach (var prop in type.GetProperties())
                {
                    var objVal = (object)null;
                    var value = jsonObj.Property(prop.Name)?.Value?.ToString() ?? string.Empty;
                    var propTypeAsString = prop.PropertyType.ToString();

                    if (prop.PropertyType.IsArray || prop.PropertyType.IsListType())
                        objVal = ExecuteMethod("CreateAndInitInstanceList", propTypeAsString, value);
                    else
                        if (!string.IsNullOrEmpty(value) && value.StartsWith('{') && value.EndsWith('}'))
                        objVal = ExecuteMethod("CreateAndInitInstance", propTypeAsString, value);

                    if (!string.IsNullOrEmpty(value) || objVal != null)
                    {
                        var valueAsType = (object)null;
                        if (propTypeAsString.ToLower().IndexOf("int32") > -1)
                            valueAsType = jsonObj.Property(prop.Name)?.Value.ToObject<int>();
                        else
                        if (propTypeAsString.ToLower().IndexOf("single") > -1 ||
                                propTypeAsString.ToLower().IndexOf("float") > -1 ||
                                    propTypeAsString.ToLower().IndexOf("double") > -1)
                            valueAsType = jsonObj.Property(prop.Name)?.Value.ToObject<float>();
                        else
                            valueAsType = value;

                        type.GetProperty(prop.Name).SetValue(instance, (objVal != null ? objVal : valueAsType));
                    }
                }
            }

            return instance;
        }
        #endregion

        #region CreateAndInitInstanceList
        public static T[] CreateAndInitInstanceList<T>(string jsonArrayAsStr)
        {
            var list = new List<T>();

            if (jsonArrayAsStr != "")
            {
                var array = JArray.Parse(jsonArrayAsStr);
                foreach (var node in array)
                {
                    if (node is JValue)
                    {
                        object obj = (node as JValue).ToString();
                        list.Add((T)obj);
                    }
                    else
                    {
                        var obj = CreateAndInitInstance<T>((JObject)node);
                        list.Add(obj);
                    }
                }
            }

            return list.ToArray();
        }
        #endregion

        #region Helpers
        private static object ExecuteMethod(string methodName, string typeAsStr, string paramValAsStr)
        {
            var isArray = false;
            if (typeAsStr.IndexOf("[]") > -1)
            {
                isArray = true;
                typeAsStr = typeAsStr.Remove(typeAsStr.IndexOf("[]"), 2);
            }

            var propertyType = Type.GetType(typeAsStr);
            var instanceType = typeof(SerializationHelper);
            var instanceMethod = instanceType.GetMethod(methodName);
            var genericMethod = instanceMethod.MakeGenericMethod(propertyType);
            var instance = new SerializationHelper();

            return genericMethod.Invoke(instance, new object[] { isArray ? paramValAsStr : (object)JObject.Parse(paramValAsStr) });
        }
        #endregion
    }
}
