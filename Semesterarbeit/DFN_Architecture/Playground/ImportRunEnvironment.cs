using System.IO;
using System.Threading;
using Common.Logging;
using Dfn.Etl.Core;
using Dfn.Etl.Crosscutting.Logging;
using DFN_Architecture.Playground.Actions;
using DFN_Architecture.Playground.Params;
using LogLevel = Dfn.Etl.Crosscutting.Logging.LogLevel;

namespace DFN_Architecture.Playground
{
    /// <summary>
    /// Represents an object that encapsulates the logic required to initialize the environment for a task and to actually run the task.
    /// The main purpose of this object is to create the ImportPipeline-Class smaller.
    /// </summary>
    public static class ImportRunEnvironment
    {
        private static readonly ILog s_Log = LogManager.GetLogger("PGImportRunEnvironmentPipeline");

        public static ILog Log
        {
            get
            {
                return s_Log;
            }
        }


        public static void RunPlayGroundImport(CancellationTokenSource cts)
        {

            var stockParams = new Pg_Params
            {
                BatchQueueSize = 10,
                //new AbstractedEnum(CustomerImportTypeEnum.Stock)
                //                          .GetMappedId(ImportPipeline.ImportConfiguration.CustomerImporttypesQueueSizeBatchTaskMapping),
                CommonQueueSize = 10,
                //new AbstractedEnum(CustomerImportTypeEnum.Stock)
                //                          .GetMappedId(ImportPipeline.ImportConfiguration.CustomerImporttypesQueueSizeCommonTaskMapping),
                PersistQueueSize = 2,
                //new AbstractedEnum(CustomerImportTypeEnum.Stock)
                //                          .GetMappedId(ImportPipeline.ImportConfiguration.CustomerImporttypesQueueSizePersistTaskMapping),
                ImportFolder = @"C:\Devel\Fsharp\Mercurial\tmp"//,
                //new AbstractedEnum(CustomerImportTypeEnum.Stock)
                //                          .GetMappedValue(ImportPipeline.ImportConfiguration.CustomerImporttypesFolderFileMapping),
                //ImportType = CustomerImportTypeEnum.Stock
            };

            RunPlayGroundImport(cts, stockParams);

        }

        public static void RunPlayGroundImport(CancellationTokenSource cts, Pg_Params parms)
        {
            //var importType = parms.ImportType;
            string dfnName = "Dataflow Network Playground Task";

            //using (var network = new DataflowNetwork(dfnName, new ConsoleLogAgent(LogLevel.Trace))) - Slower than CommonLogAgent!!
            using (var network = new DataflowNetwork(dfnName, new CommonLogAgent(Log)))
            //, new CommonLogAgent(ImportPipeline.Log)))
            {
                var importFolderPG = parms.ImportFolder;
                var commonQueueSize = 10;//-1;//parms.CommonQueueSize * 100;
                var persistQueueSize = parms.PersistQueueSize;
                var batchQueueSize = parms.BatchQueueSize;

                var seqsrc =    network.CreateSource(new SequenceSourcer(20000), commonQueueSize);
                var seqenrch = network.CreateTransform(new SequenceEnricher(), commonQueueSize);
                var seqnoop = network.CreateTransform(new SequenceNoopAnalyzer(Log), commonQueueSize);
                var seqlggr = network.CreateTransform(new SequenceLogger(Log), commonQueueSize);
                //var seqwrt = network.CreateTransform(new SequenceWriter(Path.Combine(parms.ImportFolder, "test.txt")));
                var seqfnlz = network.CreateTarget(new SequenceFinalizer(Log));

                //network.CreateDelayingTransformMany()
                //network.CreateTargetBatched()
                //network.CreateTransformBatched()
                //network.CreateBatch<>()
                
                network.Link(seqsrc, seqenrch);
                network.Link(seqenrch, seqnoop);
                network.Link(seqnoop, seqlggr);
                //network.Link(seqlggr, seqwrt);
                network.Link(seqlggr, seqfnlz);

               // network.Link(seqsrc, seqfnlz);
                //network.Link(seqnoop, seqfnlz);

                /// ToDo: get rid of this hack
                ///http://zguide.zeromq.org/page:all 
                ///check for slow joiner, tcp connection could probably not fully established while sending messages. 
                ///hacky workaround, just wait a sec :))
                Thread.Sleep(1000);
                network.Start(seqsrc);
            }
        }

    }
}