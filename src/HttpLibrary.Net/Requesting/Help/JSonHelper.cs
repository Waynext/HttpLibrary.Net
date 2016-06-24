// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using HttpLibrary.Common;

namespace HttpLibrary.Requesting.Help
{
    class JSonHelper
    {
        /// <summary>
        /// Get JObject property value
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="propNames">/parent/child</param>
        /// <returns></returns>
        public static T GetJObjectPropValue<T>(JObject jObj, string propNames) where T : class
        {
            T result = default(T);
            if (jObj.IsNull() || propNames.IsNullOrEmptyOrWhiteSpace())
            {
                return result;
            }

            string[] propNamesList = propNames.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            JObject jSource = jObj;
            int itemLen = propNamesList.Length;
            for (int i = 0; i < itemLen; i++)
            {
                string item = propNamesList[i];
                JToken jToken;
                if (!jSource.IsNull() && jSource.TryGetValue(item, out jToken))
                {
                    jSource = jToken as JObject;
                    T jValue = jToken as T;
                    if (!jValue.IsNull() && (i == itemLen - 1))
                    {
                        result = jValue;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Get JObject property value
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="propNames">/parent/child</param>
        /// <returns></returns>
        static public string GetJObjectPropStringValue(JObject jObj, string propNames)
        {
            string result = null;
            JValue jValue = GetJObjectPropValue<JValue>(jObj, propNames);
            if (jValue != null && jValue.Value != null)
            {
                result = jValue.Value.ToString();
            }
            return result;
        }

        /// <summary>
        /// Get JObject property value
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="propNames">/parent/child</param>
        /// <returns></returns>
        static public long GetJObjectPropLongValue(JObject jObj, string propNames)
        {
            long result = 0;
            JValue jValue = GetJObjectPropValue<JValue>(jObj, propNames);
            if (!jValue.IsNull() && !jValue.Value.IsNull())
            {
                result = long.Parse(jValue.Value.ToString());
            }
            else
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// Get JObject property value, if property not existed , return -1
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="propNames">/parent/child</param>
        /// <returns></returns>
        static public int GetJObjectPropIntValue(JObject jObj, string propNames)
        {
            int result = 0;
            JValue jValue = GetJObjectPropValue<JValue>(jObj, propNames);
            if (!jValue.IsNull() && !jValue.Value.IsNull())
            {
                result = int.Parse(jValue.Value.ToString());
            }
            else
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// Get JObject property value, if property not existed , return -1
        /// </summary>
        /// <param name="jObj"></param>
        /// <param name="propNames">/parent/child</param>
        /// <returns></returns>
        static public bool GetJObjectPropBoolValue(JObject jObj, string propNames)
        {
            bool result = false;
            JValue jValue = GetJObjectPropValue<JValue>(jObj, propNames);
            if (!jValue.IsNull() && !jValue.Value.IsNull())
            {
                result = bool.Parse(jValue.Value.ToString());
            }

            return result;
        }

        public static IEnumerable<T> GetJObjectPropValues<T>(JObject jObj, string propNames) where T : class
        {
            IEnumerable<T> result = null;
            if (jObj.IsNull() || propNames.IsNullOrEmptyOrWhiteSpace())
            {
                return result;
            }

            string[] propNamesList = propNames.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            JObject jSource = jObj;
            int itemLen = propNamesList.Length;
            for (int i = 0; i < itemLen; i++)
            {
                string item = propNamesList[i];
                JToken jToken;
                if (!jSource.IsNull() && jSource.TryGetValue(item, out jToken))
                {
                    var jArray = jToken as JArray;

                    if (!jArray.IsNull())
                    {
                        result = jArray.Select(jT => jT.ToObject<T>());
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

    }
}
