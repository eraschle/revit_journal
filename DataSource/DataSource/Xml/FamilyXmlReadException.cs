using System;

namespace DataSource.Xml
{
    internal enum ExceptionStatus { Unkown, NoStart, NoEnd, NotValidData }

    internal class FamilyXmlReadException : Exception
    {

        private readonly ExceptionStatus Status;
        public FamilyXmlReadException() { }

        public FamilyXmlReadException(string message) : base(message) { }

        public FamilyXmlReadException(string message, Exception innerException) : base(message, innerException) { }

        internal FamilyXmlReadException(ExceptionStatus status) : base()
        {
            Status = status;
        }

        internal FamilyXmlReadException(ExceptionStatus status, string message) : base(message)
        {
            Status = status;
        }

        internal FamilyXmlReadException(ExceptionStatus status, string message, Exception innerException)
            : base(message, innerException)
        {
            Status = status;
        }
        internal FamilyXmlReadException(ExceptionStatus status, Exception innerException)
      : base(innerException.Message, innerException)
        {
            Status = status;
        }

        public bool IsRepairable
        {
            get
            {
                return Status == ExceptionStatus.NoEnd
                    || Status == ExceptionStatus.NotValidData;
            }
        }

    }
}
