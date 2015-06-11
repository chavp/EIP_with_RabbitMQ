using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models
{
    public class Instruction
    {
        public string Module { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
    }
}
