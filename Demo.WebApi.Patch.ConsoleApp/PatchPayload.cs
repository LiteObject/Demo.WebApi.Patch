using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.ConsoleApp
{
    public class PatchPayload
    {
        public object Value { get; set; }

        public string Path { get; set; }

        public string Op { get; set; }
    }
}
