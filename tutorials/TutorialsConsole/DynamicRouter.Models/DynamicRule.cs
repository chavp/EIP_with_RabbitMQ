using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRouter.Models
{
    public class DynamicRule
    {
        public string Name { get; set; }
        public long TotalService { get; set; }
        public TimeSpan CurrentElapsed { get; set; }
    }
}
