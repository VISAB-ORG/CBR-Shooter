namespace VISABConnector
{
    /// <summary>
    /// Event args for when the VISABApi object is being closed
    /// </summary>
    public class ClosingEventArgs
    {
        /// <summary>
        /// The RequestHandler currently in use by the VISABApi object
        /// </summary>
        public IVISABRequestHandler RequestHandler { get; set; }
    }
}