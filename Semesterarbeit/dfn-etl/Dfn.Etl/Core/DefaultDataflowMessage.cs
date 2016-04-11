using System;
using System.Collections.Generic;
using System.Linq;
using Dfn.Etl.Core.Tasks;

namespace Dfn.Etl.Core
{
    /// <summary>
    /// Default implementation of a Content / DataFlowMessage.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class DefaultDataflowMessage<TData> : IDataflowMessage<TData>
    {
        private IDataflowNetworkTask m_CreatedByTask;
        public virtual IDataflowNetworkTask CreatedByTask { get { return m_CreatedByTask; } set { m_CreatedByTask = value; } }

        private Guid m_Guid;
        public virtual Guid Guid { get { return m_Guid; } protected set { m_Guid = value; } }

        private string m_Title;
        public virtual string Title { get { return m_Title; } set { m_Title = value; } }

        private bool m_IsBroken;
        public virtual bool IsBroken { get { return m_IsBroken; } set { m_IsBroken = value; } }

        private bool m_IsEmpty;
        public virtual bool IsEmpty { get { return m_IsEmpty; } protected set { m_IsEmpty = value; } }

        private TData m_Data;
        public virtual TData Data { get { return m_Data; } protected set { m_Data = value; } }

        private IMetadata m_Metadata;
        public virtual IMetadata Metadata { get { return m_Metadata; } protected set { m_Metadata = value; } }

        private List<DataflowMessageError> m_Errors;
        public virtual IEnumerable<DataflowMessageError> Errors { get { return m_Errors; } protected set { m_Errors = value.ToList(); } }

        object IDataflowMessage.Data
        {
            get { return Data; }
        }

        public DefaultDataflowMessage()
        {
            m_Guid = System.Guid.NewGuid();
            m_Errors = new List<DataflowMessageError>();
            m_Metadata = new DictionaryMetadata();
        }

        public DefaultDataflowMessage(TData data)
        {
            m_Guid = System.Guid.NewGuid();
            m_Errors = new List<DataflowMessageError>();
            m_Metadata = new DictionaryMetadata();
            m_Data = data;
        }

        public virtual DefaultDataflowMessage<TData> WithTitle(string title)
        {
            m_Title = title;
            return this;
        }

        public virtual void SetData(object data)
        {
            if (data == null)
            {
                m_Data = default(TData);
            }
            else
            {
                m_Data = (TData)data;
            }
        }

        public virtual void AddError(DataflowMessageError e)
        {
            m_Errors.Add(e);
        }

        public virtual void CloneFrom(IDataflowMessage fromMsg)
        {
            CreatedByTask = fromMsg.CreatedByTask;
            Title = fromMsg.Title;
            Errors = fromMsg.Errors;
            IsBroken = fromMsg.IsBroken;

            if (fromMsg.Metadata != null)
            {
                Metadata = fromMsg.Metadata.Clone();
            }
            
            SetData(fromMsg.Data);
        }

        public virtual void AddChildMessage(IDataflowMessage msg)
        {
            //Set this as one of the parents for the specified message
            msg.AddParentMessage(this);

            //Add to the children
        }

        public virtual void AddParentMessage(IDataflowMessage msg)
        {
            //Add to the parents
        }
    }
}