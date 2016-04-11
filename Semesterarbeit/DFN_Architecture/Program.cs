using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DFN_Architecture.Playground;

namespace DFN_Architecture
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            ImportRunEnvironment.RunPlayGroundImport(cts);
        }
    }
}
