using System;

namespace VISABConnector
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DontSerialize : Attribute
    {
    }
}