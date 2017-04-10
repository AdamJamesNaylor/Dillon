
namespace Dillon.Common {
    using System;

    public static class ExceptionExtensions {
        public static Exception InnerMostException(this Exception e) {
            if (e.InnerException == null)
                return e;

            return e.InnerException.InnerMostException();
        }
    }
}