using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFN_Architecture.Playground.Params
{
    public class Pg_Params
    {
        public string ImportFolder { get; set; }
        //public CustomerImportTypeEnum ImportType { get; set; }
        public int CommonQueueSize { get; set; }
        public int PersistQueueSize { get; set; }
        public int BatchQueueSize { get; set; }

    }
}
