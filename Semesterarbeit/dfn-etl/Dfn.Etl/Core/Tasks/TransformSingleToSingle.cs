using System;
using System.Collections.Generic;

namespace Dfn.Etl.Core.Tasks
{
    #region SingleToSingle
    public abstract class TransformSingleToSingle<TTransform, TInput, TInputMsg, TOutput, TOutputMsg> : Transform<TTransform, TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.Transformation; }
        }

        protected TransformSingleToSingle()
        {
        }

        protected override IEnumerable<TOutputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c)
        {
            if(decoratedInputMsgs == null)
                yield break;

            //Process each message
            TOutputMsg outputMsg = default(TOutputMsg);
            TOutput outputData = default(TOutput);
            Exception ex = null;
            bool hasGeneratedOutputMsg = false;
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
                    outputMsg = TransformMessage(msg, c);
                }
                catch (Exception e)
                {
                    //Obviously overridden... this set as processed...
                    c.Processed = true;
                    ex = e;
                }

                hasGeneratedOutputMsg = !c.Processed;

                if (!hasGeneratedOutputMsg)
                {
                    //TransformMessage didn't really do anything... so try another transform version!
                    c.Processed = false;
                    var inputData = msg.Data;
                    try
                    {
                        outputData = TransformData(inputData, c);
                        hasGeneratedOutputMsg = !c.Processed;
                        if (hasGeneratedOutputMsg)
                        {
                            outputMsg = CreateOutputMessage(outputData);
                        }
                    }
                    catch (Exception e)
                    {
                        //Obviously overridden... this set as processed...
                        c.Processed = true;
                        ex = e;
                    }

                    if (!hasGeneratedOutputMsg)
                    {
                        //TransformData didn't really do anything... so try with the final version!
                        c.Processed = false;
                        try
                        {
                            outputMsg = TransformMessage(msg);
                            hasGeneratedOutputMsg = !c.Processed;
                        }
                        catch (Exception e)
                        {
                            //Obviously overridden... this set as processed...
                            c.Processed = true;
                            ex = e;
                        }
                    }
                }

                if (ex != null)
                {
                    //we have an error while processing!
                    outputMsg = CreateOutputMessageWithError(ex);
                    hasGeneratedOutputMsg = true;

                    //Log error message?
                }

                //Add as output message 
                c.OutputMessages.Add(outputMsg);

                if (!hasGeneratedOutputMsg)
                {
                    //Log empty message?
                }

                yield return outputMsg;
            }
        }

        public virtual TOutputMsg TransformMessage(TInputMsg msg, Context context)
        {
            context.Processed = true;
            return default(TOutputMsg);
        }

        public virtual TOutput TransformData(TInput inputData, Context context)
        {
            context.Processed = true;
            return default(TOutput);
        }

        public virtual TOutputMsg TransformMessage(TInputMsg msg)
        {
            var outputData = TransformData(msg.Data);

            //Create the message per default
            return CreateOutputMessage(outputData);
        }

        public virtual TOutput TransformData(TInput inputData)
        {
            return default(TOutput);
        }
    }

    public abstract class TransformSingleToSingle<TTransform, TInput, TOutput> : TransformSingleToSingle<TTransform, TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>, ITransformationTask<TInput, TOutput>
    {
        public override IDataflowMessage<TOutput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TOutput>();
        }
    }
    #endregion
}