using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfn.Etl.Core.Tasks
{
    #region ManyToSingle
    public abstract class TransformManyToSingle<TTransform, TInput, TInputMsg, TOutput, TOutputMsg> : Transform<TTransform, TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.TransformationBatched; }
        }

        protected TransformManyToSingle()
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
            
            //Add the specified msg as an input message

            var msgs = decoratedInputMsgs.ToList();

            c.ActivInputMessage = msgs.FirstOrDefault();
            c.InputMessages.AddRange(msgs);
            c.ActiveInputMessages.AddRange(msgs);

            c.Processed = false;
            try
            {
                outputMsg = TransformMessages(msgs, c);
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
                var inputData = msgs.Select(msg => msg.Data);
                try
                {
                    outputData = TransformDatas(inputData, c);
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
                        outputMsg = TransformMessages(msgs);
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

        public virtual TOutputMsg TransformMessages(IEnumerable<TInputMsg> msgs, Context context)
        {
            context.Processed = true;
            return default(TOutputMsg);
        }

        public virtual TOutput TransformDatas(IEnumerable<TInput> inputData, Context context)
        {
            context.Processed = true;
            return default(TOutput);
        }

        public virtual TOutputMsg TransformMessages(IEnumerable<TInputMsg> msgs)
        {
            var datas = msgs.Select(msg => msg.Data);

            var outputDatas = TransformDatas(datas);

            //Create the message per default
            return CreateOutputMessage(outputDatas);
        }

        public virtual TOutput TransformDatas(IEnumerable<TInput> inputData)
        {
            return default(TOutput);
        }
    }

    public abstract class TransformManyToSingle<TTransform, TInput, TOutput> : TransformManyToSingle<TTransform, TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>, ITransformationTask<TInput, TOutput>
    {
        public override IDataflowMessage<TOutput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TOutput>();
        }
    }
    #endregion
}
