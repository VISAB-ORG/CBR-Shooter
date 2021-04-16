using System;

namespace VISABConnector
{
    public interface IVISABStatistics
    {
        public string Game { get; }

        public Guid SessionId { get; }
    }
}