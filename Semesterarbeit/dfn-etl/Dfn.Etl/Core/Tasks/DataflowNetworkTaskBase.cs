using System;
using System.Collections.Generic;

namespace Dfn.Etl.Core.Tasks
{
    public abstract class DataflowNetworkTaskBase<TTask, TInput, TInputMsg, TOutput, TOutputMsg> : IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg>, ILockableTask
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        private bool m_IsFirstExecution = true;

        private object m_TaskSyncObj = new object();
        public object SyncObj { get { return m_TaskSyncObj; } }

        private string m_Name;
        public virtual string Name { get { return m_Name; } protected set { m_Name = value; } }

        public abstract DataflowNetworkConstituent TaskType { get; }

        public bool IsLocked { get { return LockedBy != null; } }
        public TaskLock LockedBy { get; set; }

        #region Ctor
        protected DataflowNetworkTaskBase(string name = null)
        {
            m_Name = name;
            if (m_Name == null)
            {
                m_Name = GetType().Name;
            }
        }
        #endregion

        public abstract TOutputMsg CreateOutputMessage();

        #region Lockable
        
        protected IDisposable LockTask()
        {
            return new TaskLock(this);
        }
        #endregion

        #region Task start / end
        public void BeginProcessingTask()
        {
            using (LockTask())
            {
                //Do safely update the task
                BeginTask();
            }
        }

        protected virtual void BeginTask()
        {
            //Safe to make updates to task properties
        }

        public void EndProcessingTask()
        {
            using (LockTask())
            {
                //Do safely update the task
                EndTask();
            }
        }

        protected virtual void EndTask()
        {
            //Safe to make updates to task properties
        }
        #endregion

        #region Context start / end
        private void BeginProcessingContext(Context c)
        {
            //Begin for the context
            using (LockTask())
            {
                if (m_IsFirstExecution)
                {
                    m_IsFirstExecution = false;
                    //Start processin the task
                    BeginProcessingTask();
                }

                //Do safely update
                BeginContext(c);
            }
        }

        protected virtual void BeginContext(Context context)
        {
            //Safe to make updates to task properties
        }

        private void EndProcessingContext(Context c)
        {
            //End for the context - the end method is synchronized...
            using (LockTask())
            {
                EndContext(c);
            }
        }

        protected virtual void EndContext(Context context)
        {
            //Safe to make updates to task properties
        }
        #endregion

        #region Process
        public IEnumerable<TOutputMsg> Process(IEnumerable<TInputMsg> inputMsgs)
        {
            Context c;
            try
            {
                c = CreateContext();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create context for task.", e);
            }

            try
            {
                BeginProcessingContext(c);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to initialize task context.", e);
            }

            IEnumerable<TInputMsg> decoratedInputMsgs = null;
            try
            {
                decoratedInputMsgs = DecorateInputMessages(inputMsgs, c);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to decorate input messages for task.", e);
            }
            
            IEnumerable<TOutputMsg> outputMsgs = null;
            try
            {
                outputMsgs = ProcessMessages(decoratedInputMsgs, c);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to process messages for task.", e);
            }

            IEnumerable<TOutputMsg> decoratedOutputMsgs = null;
            try
            {
                decoratedOutputMsgs = DecorateOutputMessages(outputMsgs, c);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to decorate output messages for task.", e);
            }

            try
            {
                EndProcessingContext(c);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to finalize processing task context.");
            }

            return decoratedOutputMsgs;
        }

        protected abstract IEnumerable<TOutputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c);

        #endregion

        #region Message

        protected virtual IEnumerable<TInputMsg> DecorateInputMessages(IEnumerable<TInputMsg> inputMsgs, Context context)
        {
            return inputMsgs;
        }

        protected virtual IEnumerable<TOutputMsg> DecorateOutputMessages(IEnumerable<TOutputMsg> outputMsgs, Context context)
        {
            return outputMsgs;
        }

        protected virtual IDataflowMessage<string> DecorateOutputMessage(IDataflowMessage<string> msg, Context context)
        {
            //Copy all the metadatas from the active input messages, and set the parent / child relations
            foreach (var fromMsg in context.ActiveInputMessages)
            {
                //Set all the metadata from the message
                msg.Metadata.SetAll(fromMsg.Metadata);

                //Add as child message
                fromMsg.AddChildMessage(msg);
            }

            return msg;
        }

        public virtual void FinalizeInputMessage(TInputMsg msg)
        {
        }

        public virtual void FinalizeOutputMessage(TOutputMsg msg)
        {
        }
        #endregion


        #region Helper methods

        public virtual TOutputMsg CreateOutputMessage(TOutput outputData)
        {
            var outputMsg = CreateOutputMessage();
            outputMsg.SetData(outputData);
            outputMsg.Title = Name;
            return outputMsg;
        }


        protected virtual TOutputMsg CreateOutputMessageFor(TOutputMsg fromMsg)
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

        protected virtual TOutputMsg CreateOutputMessageWithError(Exception exception)
        {
            var msg = CreateOutputMessage();
            //Add the error
            msg.AddError(DataflowMessageError.For(exception));

            //Set the message as broken
            msg.IsBroken = true;

            return msg;
        }

        protected virtual TInputMsg CreateInputMessageWithErrorFor(TInputMsg msg, Exception exception)
        {
            //Add the error
            msg.AddError(DataflowMessageError.For(exception));

            //Set the message as broken
            msg.IsBroken = true;

            return msg;
        }

        protected virtual Context CreateContext()
        {
            return new Context();
        }
        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;

            return GetType().Name;
        }

        public virtual void Dispose()
        {
        }

        #region Helper classes
        public class Context
        {
            //Only for internal use... a bit of hack...
            internal bool Processed { get; set; }

            public List<TInputMsg> InputMessages { get; protected set; }
            public List<TOutputMsg> OutputMessages { get; protected set; }

            public TInputMsg ActivInputMessage { get; set; }
            public List<TInputMsg> ActiveInputMessages { get; protected set; }

            public Context()
            {
                InputMessages = new List<TInputMsg>();
                OutputMessages = new List<TOutputMsg>();
                ActiveInputMessages = new List<TInputMsg>();
            }

            public virtual void Apply(Context context)
            {
                //Copy some specific data
                InputMessages.AddRange(context.InputMessages);
                OutputMessages.AddRange(context.OutputMessages);
            }
        }

        #endregion
    }
}