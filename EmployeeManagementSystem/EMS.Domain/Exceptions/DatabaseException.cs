using System;

namespace EMS.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a database operation fails
    /// </summary>
    public class DatabaseException : Exception
    {
        public string Operation { get; }

        /// <summary>
        /// Creates a new DatabaseException with operation name and custom message
        /// </summary>
        public DatabaseException(string operation, string message)
            : base($"Database operation '{operation}' failed: {message}")
        {
            Operation = operation;
        }

        /// <summary>
        /// Creates a new DatabaseException with operation name and inner exception
        /// </summary>
        public DatabaseException(string operation, Exception innerException)
            : base($"Database operation '{operation}' failed: {innerException.Message}", innerException)
        {
            Operation = operation;
        }

        /// <summary>
        /// Creates a new DatabaseException with just a message
        /// </summary>
        public DatabaseException(string message) : base(message)
        {
            Operation = string.Empty;
        }
    }
}

