using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfn.Etl.Core.Tasks
{
    #region Source
    public interface ISourceTask<TOutput, TOutputMsg> : IDataflowNetworkTask<TOutput, TOutputMsg, TOutput, TOutputMsg>
        where TOutputMsg : IDataflowMessage<TOutput>
    {
    }

    public interface ISourceTask<TOutput> : 
        ISourceTask<TOutput, IDataflowMessage<TOutput>>,
        IDataflowNetworkTask<TOutput, TOutput>
    {
    }

    public abstract class Source<TSource, TInput, TInputMsg> : DataflowNetworkTaskBase<TSource, TInput, TInputMsg, TInput, TInputMsg>, ISourceTask<TInput, TInputMsg>
        where TInputMsg : IDataflowMessage<TInput>
    {
        public override DataflowNetworkConstituent TaskType
        {
            get { return DataflowNetworkConstituent.Source; }
        }

        protected Source()
        {
        }

        protected override IEnumerable<TInputMsg> ProcessMessages(IEnumerable<TInputMsg> decoratedInputMsgs, Context c)
        {
            //We don't care about the input messages!
            //Perhaps we should at least log something...
            IEnumerable<TInputMsg> outputMsgs = null;
            IEnumerable<TInput> outputDatas = null;
            bool hasGeneratedOutputMsgs = false;

            c.Processed = false;
            try
            {
                outputMsgs = PullMessages(c);
            }
            catch (Exception e)
            {
                //Obviously overridden... this set as processed...
                throw;
            }

            hasGeneratedOutputMsgs = !c.Processed;

            if (!hasGeneratedOutputMsgs)
            {
                //TransformMessage didn't really do anything... so try another transform version!
                c.Processed = false;
                try
                {
                    outputDatas = PullDatas(c);

                    hasGeneratedOutputMsgs = !c.Processed;

                    //Make sure it was overridden
                    if (hasGeneratedOutputMsgs)
                    {
                        //Setup the output messages
                        outputMsgs = outputDatas.Select(outputData => {
                            return CreateOutputMessage(outputData);
                        });
                    }
                }
                catch (Exception e)
                {
                    //Obviously overridden... this set as processed...
                    throw;
                }

                if (!hasGeneratedOutputMsgs)
                {
                    //TransformData didn't really do anything... so try with the final version!
                    c.Processed = false;
                    try
                    {
                        outputMsgs = PullMessages();
                    }
                    catch (Exception e)
                    {
                        //Obviously overridden... this set as processed...
                        throw;
                    }
                }
            }

            return outputMsgs;
        }

        protected virtual IEnumerable<TInputMsg> PullMessages(Context context)
        {
            context.Processed = true;
            return null;
        }

        protected virtual IEnumerable<TInputMsg> PullMessages()
        {
            var pulledData = PullDatas();
            
            if(pulledData == null)
                yield break;

            foreach (var data in pulledData)
            {
                yield return CreateOutputMessage(data);
            }
        }

        protected virtual IEnumerable<TInput> PullDatas(Context context)
        {
            context.Processed = true;
            return null;
        }

        protected virtual IEnumerable<TInput> PullDatas()
        {
            return null;
        }
    }

    public abstract class Source<TSource, TInput> : Source<TSource, TInput, IDataflowMessage<TInput>>, ISourceTask<TInput>
    {
        public override IDataflowMessage<TInput> CreateOutputMessage()
        {
            return new DefaultDataflowMessage<TInput>();
        }
    }
    #endregion
}
