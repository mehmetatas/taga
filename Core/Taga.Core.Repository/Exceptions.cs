using System;

namespace Taga.Core.Repository
{
    public class RepositoryException : ApplicationException
    {
        internal RepositoryException(string message)
            : base(message)
        {
        }

        internal RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class TransactionAlreadyCompletedException : RepositoryException
    {
        internal TransactionAlreadyCompletedException()
            : base("Transaction has already been completed")
        {
        }
    }

    public class TransactionAlreadyStartedException : RepositoryException
    {
        internal TransactionAlreadyStartedException()
            : base("A transaction has already been started")
        {
        }
    }

    public class TransactionNotStartedException : RepositoryException
    {
        internal TransactionNotStartedException()
            : base("Transaction has not been started")
        {
        }
    }
}