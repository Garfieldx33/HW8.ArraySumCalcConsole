using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW8.ArraySumCalcConsole
{
    public class TaskInfo
    {
        public int ItemsCount { get; set; }
        public int TreadsCount { get; set; }
        public string CalcType { get; set; }
        public double CalcTimeInMs { get; set; }
        public long TotalSum { get; set; }
    }
}
