using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeWorkloadService.Crosscutting
{
	public class MsgContainer : ICloneable
	{
		public readonly WorkloadStatisticsContext WorkloadStatistics;
		public readonly UInt64 NmsReceived;

		public MsgContainer(WorkloadStatisticsContext workloadStatistics, UInt64 nmsRec)
		{
			WorkloadStatistics = workloadStatistics;
			NmsReceived = nmsRec;
		}

		public object Clone()
		{
			return new MsgContainer((WorkloadStatisticsContext)WorkloadStatistics.Clone(), this.NmsReceived);
		}
	}
}
