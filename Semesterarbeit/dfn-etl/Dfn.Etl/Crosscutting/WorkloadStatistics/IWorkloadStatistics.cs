//using NetMQ;
using RealtimeWorkloadService.Crosscutting;

namespace Dfn.Etl.Crosscutting.WorkloadStatistics
{
    interface IWorkloadStatistics
    {
        //void LoadStatistics();
        //NetMQSocket GetClientSocket();
        void Send(WorkloadStatisticsContext wsctx);
        int GetBoundedCapacity();
    }
}
