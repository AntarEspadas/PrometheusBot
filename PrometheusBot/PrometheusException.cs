using System;

namespace PrometheusBot
{

    [Serializable]
    public class PrometheusException : Exception
    {
        public PrometheusException() { }
        public PrometheusException(string message) : base(message) { }
        public PrometheusException(string message, Exception inner) : base(message, inner) { }
        protected PrometheusException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
