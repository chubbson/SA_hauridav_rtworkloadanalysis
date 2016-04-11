using System;
using System.Collections.Generic;

namespace Dfn.Etl.Core.Tasks
{
    #region TargetPerSingle
    public abstract class TargetPerSingle<TTarget, TInput, TInputMsg> : DataflowNetworkTaskBase<TTarget, TInput, TInputMsg, TInput, TInputMsg>, ITargetTask<TInput, TInputMsg> 
        where TInputMsg : IDataflowMessage<TInput>
    {
        public override DataflowNetworkConstituent TaskType { get { return DataflowNetworkConstituent.Target; } }

        protected TargetPerSingle()
        {
        }

        protected override IEnumerable<TInputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c)
        {
            if (decoratedInputMsgs == null)
                yield break;

            Exception ex = null;
            bool hasProcessedMsg = false;
            foreach (var msg in decoratedInputMsgs)
            {
                //Add the specified msg as an input message
                c.InputMessages.Add(msg);
                //Set the active message/s
                c.ActivInputMessage = msg;
                c.ActiveInputMessages.Add(msg);

                c.Processed = false;
                try
                {
                    ExecuteForMessage(msg, c);
                }
                catch (Exception e)
                {
                    //Obviously overridden... this set as processed...
                    c.Processed = true;
                    ex = e;
                }

                hasProcessedMsg = !c.Processed;

                if (!hasProcessedMsg)
                {
                    //TransformMessage didn't really do anything... so try another transform version!
                    c.Processed = false;
                    var inputData = msg.Data;
                    try
                    {
                        ExecuteForData(inputData, c);
                        hasProcessedMsg = !c.Processed;
                    }
                    catch (Exception e)
                    {
                        //Obviously overridden... this set as processed...
                        c.Processed = true;
                        ex = e;
                    }

                    if (!hasProcessedMsg)
                    {
                        //TransformData didn't really do anything... so try with the final version!
                        c.Processed = false;
                        try
                        {
                            ExecuteForMessage(msg);
                            hasProcessedMsg = !c.Processed;
                        }
                        catch (Exception e)
                        {
                            //Obviously overridden... this set as processed...
                            c.Processed = true;
                            ex = e;
                        }
                    }
                }

                if(ex != null)
                {
                    throw ex;
                }
            }

            yield break;
        }

        public virtual void ExecuteForMessage(TInputMsg msg, Context context)
        {
            context.Processed = true;
        }

        public virtual void ExecuteForData(TInput inputData, Context context)
        {
            context.Processed = true;
        }

        public virtual void ExecuteForMessage(TInputMsg msg)
        {
            ExecuteForData(msg.Data);
        }

        public virtual void ExecuteForData(TInput inputData)
        {
        }
    }

    public abstract class TargetPerSingle<TTarget, TInput> : TargetPerSingle<TTarget, TInput, IDataflowMessage<TInput>>, ITargetTask<TInput>
    {
        public override IDataflowMessage<TInput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TInput>();
        }
    }
    #endregion
}
