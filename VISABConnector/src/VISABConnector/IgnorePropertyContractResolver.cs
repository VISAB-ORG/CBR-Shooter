using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace VISABConnector
{
    /// <summary>
    /// Contract resolver that doesn't serialize properties decorated with attributes of type T.
    /// </summary>
    /// <typeparam name="T">The attribute type to ignore</typeparam>
    public class IgnorePropertyContractResolver<T> : DefaultContractResolver where T : Attribute
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttribute(typeof(T)) != null)
                property.ShouldSerialize = x => false;

            return property;
        }
    }
}