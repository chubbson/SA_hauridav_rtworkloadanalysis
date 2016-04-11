using System;
using System.Collections.Generic;
using Dfn.Etl.Core;
using Dfn.Etl.Core.Tasks;
using Dfn.Etl.Crosscutting.Logging;

namespace Dfn.Etl.Crosscutting.ExceptionHandling
{
    public class ExceptionDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg> : DataflowNetworkDecoratorTask<TInput, TInputMsg, TOutput, TOutputMsg>
        where TOutputMsg : IDataflowMessage<TOutput>
        where TInputMsg : IDataflowMessage<TInput>
    {
        public ExceptionDecoratorTask(IDataflowNetworkTask<TInput, TInputMsg, TOutput, TOutputMsg> innerTask, IIdasDataflowNetwork network, ILogAgent logAgent)
            : base(innerTask, network, logAgent)
        {
        }

        protected override IEnumerable<TOutputMsg> ProcessInner(IEnumerable<TInputMsg> inputMsgs, out bool hasErrors)
        {
            TOutputMsg errorResult = default(TOutputMsg);
            try
            {
                return base.ProcessInner(inputMsgs, out hasErrors);
            }
            catch (DataflowNetworkUnrecoverableErrorException ex)
            {
                LogAgent.LogFatal(TaskType, InnerTask.Name, ex);

                //Now cancel the network
                Network.CancelNetwork();

                errorResult = CreateErrorOutputMessage(ex);
            }
            catch (DataflowNetworkRecoverableErrorException ex)
            {
                LogAgent.LogUnknown(TaskType, InnerTask.Name, ex);

                errorResult = CreateErrorOutputMessage(ex);
            }
            catch (Exception ex)
            {
                errorResult = CreateErrorOutputMessage(ex);
            }

            hasErrors = true;

            return new[] {errorResult};
        }

        protected override IEnumerable<TOutputMsg> DecorateOutputMessages(IEnumerable<TOutputMsg> outputMsgs)
        {
            if(outputMsgs == null)
                yield break;

            TOutputMsg errorResult = default(TOutputMsg);
            var e = outputMsgs.GetEnumerator();
            while (true)
            {
                try
                {
                    if(!e.MoveNext())
                        yield break;
                }
                catch (DataflowNetworkUnrecoverableErrorException ex)
                {
                    LogAgent.LogFatal(TaskType, InnerTask.Name, ex);

                    //Now cancel the network
                    Network.CancelNetwork();

                    errorResult = CreateErrorOutputMessage(ex);
                }
                catch (DataflowNetworkRecoverableErrorException ex)
                {
                    LogAgent.LogUnknown(TaskType, InnerTask.Name, ex);

                    errorResult = CreateErrorOutputMessage(ex);
                }
                catch (Exception ex)
                {
                    errorResult = CreateErrorOutputMessage(ex);
                }

                if (errorResult != null)
                {
                    yield return errorResult;
                }

                var current = e.Current;
                yield return current;
            }
        }
    }
}