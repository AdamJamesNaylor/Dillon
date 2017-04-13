namespace Dillon.Common {
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UnableToCreateDirectoryException
        : Exception {

        public UnableToCreateDirectoryException() {
        }

        public UnableToCreateDirectoryException(string message)
            : base(message) {
        }

        public UnableToCreateDirectoryException(string message, Exception inner)
            : base(message, inner) {
        }

        protected UnableToCreateDirectoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}