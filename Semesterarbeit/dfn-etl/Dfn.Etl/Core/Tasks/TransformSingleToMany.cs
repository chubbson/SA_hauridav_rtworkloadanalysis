using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfn.Etl.Core.Tasks
{
    #region SingleToMany
    public abstract class TransformSingleToMany<TTransform, TInput, TInputMsg, TOutput, TOutputMsg> : Transform<TTransform, TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.TransformMany; }
        }

        protected TransformSingleToMany()
        {
        }

        protected override IEnumerable<TOutputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c)
        {
            if (decoratedInputMsgs == null)
                yield break;

            //Process each message
            IEnumerable<TOutputMsg> outputMsgs = null;
            IEnumerable<TOutput> outputDatas = null;
            Exception ex = null;
            bool hasGeneratedOutputMsgs = false;
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
                    outputMsgs = TransformMessage(msg, c);
                }
                catch (Exception e)
                {
                    //Obviously overridden... this set as processed...
                    c.Processed = true;
                    ex = e;
                }

                hasGeneratedOutputMsgs = !c.Processed;

                if (!hasGeneratedOutputMsgs)
                {
                    //TransformMessage didn't really do anything... so try another transform version!
                    c.Processed = false;
                    var inputData = msg.Data;
                    try
                    {
                        outputDatas = TransformData(inputData, c);
                        hasGeneratedOutputMsgs = !c.Processed;
                        if (hasGeneratedOutputMsgs)
                        {
                            outputMsgs = outputDatas.Select(outputData => {
                                return CreateOutputMessage(outputData);
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        //Obviously overridden... this set as processed...
                        c.Processed = true;
                        ex = e;
                    }

                    if (!hasGeneratedOutputMsgs)
                    {
                        //TransformData didn't really do anything... so try with the final version!
                        c.Processed = false;
                        try
                        {
                            outputMsgs = TransformMessage(msg);
                            hasGeneratedOutputMsgs = !c.Processed;
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
                    outputMsgs = new TOutputMsg[] {CreateOutputMessageWithError(ex)};
                    hasGeneratedOutputMsgs = true;
                    //Log error message?
                }

                if (!hasGeneratedOutputMsgs)
                {
                    //Log empty message?
                }

                if(outputMsgs == null)
                    continue;

                //Output all the many msgs
                foreach (var outputMsg in outputMsgs)
                {
                    //Add as output message 
                    c.OutputMessages.Add(outputMsg);

                    yield return outputMsg;
                }
            }
        }

        public virtual IEnumerable<TOutputMsg> TransformMessage(TInputMsg msg, Context context)
        {
            context.Processed = true;
            return null;
        }

        public virtual IEnumerable<TOutput> TransformData(TInput inputData, Context context)
        {
            context.Processed = true;
            return null;
        }

        public virtual IEnumerable<TOutputMsg> TransformMessage(TInputMsg msg)
        {
            var outputDatas = TransformData(msg.Data);

            if(outputDatas == null)
                yield break;

            foreach (var outputData in outputDatas)
            {
                yield return CreateOutputMessage(outputData);
            }
        }

        public virtual IEnumerable<TOutput> TransformData(TInput inputData)
        {
            return null;
        }
    }

    public abstract class TransformSingleToMany<TTransform, TInput, TOutput> : TransformSingleToMany<TTransform, TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>, ITransformationTask<TInput, TOutput>
    {
        public override IDataflowMessage<TOutput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TOutput>();
        }
    }
    #endregion
}
