using System;

namespace EMS.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when attempting to create a duplicate resource
    /// </summary>
    public class DuplicateException : Exception
    {
        public string ResourceName { get; }
        public string PropertyName { get; }
        public object PropertyValue { get; }

        public DuplicateException(string resourceName, string propertyName, object propertyValue)
            : base($"{resourceName} with {propertyName} '{propertyValue}' already exists.")
        {
            ResourceName = resourceName;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        public DuplicateException(string message) : base(message)
        {
            ResourceName = string.Empty;
            PropertyName = string.Empty;
            PropertyValue = string.Empty;
        }

        public DuplicateException(string message, Exception innerException)
            : base(message, innerException)
        {
            ResourceName = string.Empty;
            PropertyName = string.Empty;
            PropertyValue = string.Empty;
        }
    }
}

