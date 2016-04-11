using System;

namespace RealtimeWorkloadService.Crosscutting
{
    [Serializable]
    public class WorkloadStatisticsContext : ICloneable
    {
        public readonly Guid GroupGuid;
        public readonly Guid TaskGuid;
        //private OrdinalNumbersType taskNr;
        public readonly int NumberOfMessagesSeen;
        public readonly int IncQueueMsgCnt;
        public readonly int OutQueueMsgCnt;
        public readonly int BoundedCapacityCnt;
        public readonly string Context;

        public WorkloadStatisticsContext(Guid groupGuid, Guid taskGuid, int numberOfMessagesSeen, int incQueueMsgCnt, int outQueueuMsgCnt, int boundedCapacityCnt, string context)
        {
            GroupGuid = groupGuid;
            TaskGuid = taskGuid;
            NumberOfMessagesSeen = numberOfMessagesSeen;
            IncQueueMsgCnt = incQueueMsgCnt;
            OutQueueMsgCnt = outQueueuMsgCnt;
            BoundedCapacityCnt = boundedCapacityCnt;
            Context = context;
        }

        public static bool TryParse(Guid groupGuid, Guid taskGuid, string workloadStatisticsCtxString, out WorkloadStatisticsContext wlStatCtx)
        {
            try
            {
                var splittedValue = workloadStatisticsCtxString.Split('|');
                var numberOfMessagesSeen = int.Parse(splittedValue[0].Split(':')[1]);
                var boundedCapacityCnt = int.Parse(splittedValue[1].Split(':')[1]);
                var incQueueMsgCnt = int.Parse(splittedValue[2].Split(':')[1]);
                var outQueueMsgCnt = int.Parse(splittedValue[3].Split(':')[1]);
                var context = splittedValue[4];
                wlStatCtx  = new WorkloadStatisticsContext(groupGuid, taskGuid, numberOfMessagesSeen, incQueueMsgCnt, outQueueMsgCnt, boundedCapacityCnt, context);
                return true;
            }
            catch (Exception ex)
            {
                //throw new ArgumentException("stringparsing failes, allowed pattern: {guid_group}_{guid_task}#NMS: {int}|BC: {int}|IncMsg: {int}|OutMsg: {int}|Title: {string}", ex);
                throw new ArgumentException("stringparsing failes, allowed pattern: NMS: {int}|BC: {int}|IncMsg: {int}|OutMsg: {int}|Title: {string}", ex);
            }
        }

        public string GetKey()
        {
            return string.Format("{0}_{1}", GroupGuid, TaskGuid);
        }

        public override string ToString()
        {
            var m = string.Format("NMS: {0}|BC: {1}|IncMsg: {2}|OutMsg: {3}|Title: {4}", NumberOfMessagesSeen, BoundedCapacityCnt, IncQueueMsgCnt, OutQueueMsgCnt, Context);
            //var m = string.Format("{6}_{5}#NMS: {0}|BC: {1}|IncMsg: {2}|OutMsg: {3}|Title: {4}", NumberOfMessagesSeen, BoundedCapacityCnt, IncQueueMsgCnt, OutQueueMsgCnt, Context, TaskGuid, GroupGuid);
            return m; 
        }

	    public object Clone()
	    {
			return 
		    new WorkloadStatisticsContext(GroupGuid, TaskGuid, NumberOfMessagesSeen, IncQueueMsgCnt, OutQueueMsgCnt,
			    BoundedCapacityCnt, Context);
	    }
    }
}
