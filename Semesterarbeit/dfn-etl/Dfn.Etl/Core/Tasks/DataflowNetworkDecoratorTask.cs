using System;
using System.Collections.Generic;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Core.Tasks
{
    public class DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> : IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> 
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        private string m_Name;
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name))
                    return InnerTask.Name;
                return m_Name;
            }
            protected set { m_Name = value; }
        }

        public DataflowNetworkConstituent TaskType { get { return InnerTask.TaskType; } }
        public virtual void BeginProcessingTask()
        {
            InnerTask.BeginProcessingTask();
        }

        public virtual void EndProcessingTask()
        {
            InnerTask.EndProcessingTask();
        }

        private IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> m_InnerTask;
        public IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> InnerTask
        {
            get { return m_InnerTask; }
            protected set { m_InnerTask = value; }
        }

        private ILogAgent m_LogAgent;
        public virtual ILogAgent LogAgent
        {
            get { return m_LogAgent; }
            protected set { m_LogAgent = value; }
        }

        private IIdasDataflowNetwork m_Network;
        public virtual IIdasDataflowNetwork Network
        {
            get { return m_Network; }
            protected set { m_Network = value; }
        }

        public DataflowNetworkDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
        {
            if(innerTask == null)
                throw new ArgumentNullException("innerTask", "Inner task must not be null!");

            m_InnerTask = innerTask;
            m_LogAgent = logAgent;
            m_Network = network;
        }

        public IEnumerable<TOutputMsg> Process(IEnumerable<TInputMsg> inputMsgs)
        {
            bool hasErrors;
            var outputMsgs = ProcessInner(inputMsgs, out hasErrors);
            return ProcessOuter(outputMsgs, hasErrors);
        }

        protected virtual IEnumerable<TOutputMsg> ProcessInner(IEnumerable<TInputMsg> inputMsgs, out bool hasErrors)
        {
            hasErrors = false;
            var decoratedInputMsgs = DecorateInputMessages(inputMsgs);

            //Now process the inner... we have decorated the messages...
            return InnerTask.Process(decoratedInputMsgs);
        }

        protected virtual IEnumerable<TOutputMsg> ProcessOuter(IEnumerable<TOutputMsg> outputMsgs, bool hasErrors)
        {
            //Now decorate the output messages
            var decoratedOutputMsgs = DecorateOutputMessages(outputMsgs);
            return decoratedOutputMsgs;
        }

        protected virtual IEnumerable<TInputMsg> DecorateInputMessages(IEnumerable<TInputMsg> inputMsgs)
        {
            return inputMsgs;
        }

        protected virtual IEnumerable<TOutputMsg> DecorateOutputMessages(IEnumerable<TOutputMsg> outputMsgs)
        {
            return outputMsgs;
        }

        protected virtual TOutputMsg CreateErrorOutputMessage(Exception exception)
        {
            var msg = CreateOutputMessage();
            //Add the error
            msg.AddError(DataflowMessageError.For(exception));

            //Set the message as broken
            msg.IsBroken = true;

            return msg;
        }

        protected virtual TInputMsg CreateErrorInputMessage(TInputMsg msg, Exception exception)
        {
            //Add the error
            msg.AddError(DataflowMessageError.For(exception));

            //Set the message as broken
            msg.IsBroken = true;

            return msg;
        }

        public virtual TOutputMsg CreateOutputMessage()
        {
            return InnerTask.CreateOutputMessage();
        }

        protected virtual TOutputMsg CreateOutputMessageFrom(TOutputMsg fromMsg)
        {
            var msg = CreateOutputMessage();

            //Set the creator
            msg.CreatedByTask = this;

            //Clone any data
            if (fromMsg != null)
            {
                msg.CloneFrom(fromMsg);
            }

            return msg;
        }

        public override string ToString()
        {
            return InnerTask.ToString();
        }

        public virtual void Dispose()
        {
            if(InnerTask != null)
            {
                InnerTask.Dispose();
            }
        }
    }
}