using System;

namespace VISABConnector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DontSerialize : Attribute
    {
        public DontSerialize()
        {
        }
    }
}