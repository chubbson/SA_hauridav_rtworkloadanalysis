using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfn.Etl.Core.Tasks
{
    #region ManyToMany
    public abstract class TransformManyToMany<TTransform, TInput, TInputMsg, TOutput, TOutputMsg> : Transform<TTransform, TInput, TInputMsg, TOutput, TOutputMsg>
        where TInputMsg : IDataflowMessage<TInput>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.Transformation; }
        }

        protected TransformManyToMany()
        {
        }

        protected override IEnumerable<TOutputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c)
        {
            if(decoratedInputMsgs == null)
                yield break;

            //Process each message
            IEnumerable<TOutputMsg> outputMsgs = null;
            IEnumerable<TOutput> outputDatas = null;
            Exception ex = null;
            bool hasGeneratedOutputMsgs = false;
            
            //Add the specified msg as an input message

            var msgs = decoratedInputMsgs.ToList();

            c.ActivInputMessage = msgs.FirstOrDefault();
            c.InputMessages.AddRange(msgs);
            c.ActiveInputMessages.AddRange(msgs);

            c.Processed = false;
            try
            {
                outputMsgs = TransformMessages(msgs, c);
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
                var inputDatas = msgs.Select(msg => msg.Data);
                try
                {
                    outputDatas = TransformDatas(inputDatas, c);
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
                        outputMsgs = TransformMessages(msgs);
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
                outputMsgs = new TOutputMsg[] { CreateOutputMessageWithError(ex) };
                hasGeneratedOutputMsgs = true;
                //Log error message?
            }

            if (!hasGeneratedOutputMsgs)
            {
                //Log empty message?
            }

            if (outputMsgs == null)
                yield break;

            //Output all the many msgs
            foreach (var outputMsg in outputMsgs)
            {
                //Add as output message 
                c.OutputMessages.Add(outputMsg);

                yield return outputMsg;
            }
        }

        public virtual IEnumerable<TOutputMsg> TransformMessages(IEnumerable<TInputMsg> msg, Context context)
        {
            context.Processed = true;
            return null;
        }

        public virtual IEnumerable<TOutput> TransformDatas(IEnumerable<TInput> inputData, Context context)
        {
            context.Processed = true;
            return null;
        }

        public virtual IEnumerable<TOutputMsg> TransformMessages(IEnumerable<TInputMsg> msgs)
        {
            var inputDatas = msgs.Select(msg => msg.Data);

            var outputDatas = TransformDatas(inputDatas);

            if (outputDatas == null)
                yield break;

            foreach (var outputData in outputDatas)
            {
                yield return CreateOutputMessage(outputData);
            }
        }

        public virtual IEnumerable<TOutput> TransformDatas(IEnumerable<TInput> inputData)
        {
            return null;
        }
    }

    public abstract class TransformManyToMany<TTransform, TInput, TOutput> : TransformManyToMany<TTransform, TInput, IDataflowMessage<TInput>, TOutput, IDataflowMessage<TOutput>>, ITransformationTask<TInput, TOutput>
    {
        public override IDataflowMessage<TOutput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TOutput>();
        }
    }
    #endregion
}
