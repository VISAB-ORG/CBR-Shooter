using System;

namespace VISABConnector
{
    /// <summary>
    /// Properties decorated with this Attribute wont be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DontSerialize : Attribute
    {
    }
}