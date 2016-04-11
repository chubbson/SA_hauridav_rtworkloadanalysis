using System;
using System.Collections.Generic;
using System.Text;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Represents a message that flows through a dataflow network.
    /// </summary>
    public interface IDataflowMessage
    {
        /// <summary>
        /// The task that generated this message
        /// </summary>
        IDataflowNetworkTask CreatedByTask { get; set; }

        /// <summary>
        /// An id for the message
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Describes this message and its contents.
        /// This title can be used for e.g. logging purposes.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// True, if this message is marked as being broken.
        /// Broken messages are discarded.
        /// </summary>
        bool IsBroken { get; set; }

        /// <summary>
        /// True, if this message is empty, thus having no data
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Represents the actual value of this message.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Gets the metadata for this message.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IMetadata Metadata { get; }

        /// <summary>
        /// Adds an error to this message
        /// </summary>
        /// <param name="e"></param>
        void AddError(DataflowMessageError e);

        /// <summary>
        /// The errors for this message.
        /// </summary>
        IEnumerable<DataflowMessageError> Errors { get; }

        /// <summary>
        /// Clones any required data from the specified message.
        /// </summary>
        /// <param name="fromMsg"></param>
        void CloneFrom(IDataflowMessage fromMsg);

        /// <summary>
        /// Adds the specified message as an output message for this current message.
        /// </summary>
        /// <param name="childOutputMessage"></param>
        void AddChildMessage(IDataflowMessage msg);

        /// <summary>
        /// Adds the specified message as an input message for this current message.
        /// </summary>
        /// <param name="msg"></param>
        void AddParentMessage(IDataflowMessage msg);
    }

    /// <summary>
    /// Represents a message that flows through a dataflow network.
    /// </summary>
    /// <typeparam name="TData">The type of the value wrapped inside this message.</typeparam>
    public interface IDataflowMessage<out TData> : IDataflowMessage
    {
        /// <summary>
        /// Represents the actual value of this message.
        /// </summary>
        new TData Data { get; }

        /// <summary>
        /// Sets the data for this message.
        /// </summary>
        /// <param name="data"></param>
        void SetData(object data);
    }

    #region Dataflow message helper classes and structures
    public class DataflowMessageError
    {
        public static DataflowMessageError For(Exception exception, string message = null)
        {
            if (message == null && exception != null)
                message = exception.Message;

            return new DataflowMessageError() {
                Message = new ErrorMessage(message),
                Exception = exception
            };
        }

        public Exception Exception { get; set; }
        public ErrorMessage Message { get; set; }

        public DataflowMessageError()
        {
        }

        public override string ToString()
        {
            if (Exception != null)
                return Exception.ToString();

            if (Message != null)
                return Message.ToString();

            return string.Empty;
        }
    }

    public class ErrorMessage
    {
        private string m_Description;
        public virtual string Description
        {
            get { return m_Description; }
            protected set { m_Description = value; }
        }

        public ErrorMessage(string description = null)
        {
            m_Description = description;
        }

        public override string ToString()
        {
            if (m_Description == null)
                return string.Empty;

            return m_Description;
        }
    }

    public class AggregateErrorMessage : ErrorMessage
    {
        private IEnumerable<ErrorMessage> m_ErrorMessages;

        public override string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Description))
                    return base.Description;

                if (m_ErrorMessages == null)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();
                foreach (var errorMessage in m_ErrorMessages)
                {
                    sb.AppendLine(errorMessage.ToString());
                }
                base.Description = sb.ToString();
                return base.Description;
            }
            protected set { base.Description = value; }
        }

        public AggregateErrorMessage(IEnumerable<ErrorMessage> errorMessages)
        {
            m_ErrorMessages = errorMessages;
        }
    }
    #endregion
}