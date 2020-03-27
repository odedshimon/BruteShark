using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkCli
{
    class CliShellCommand
    {
        public string Keyword { get; set; }
        public string Description { get; set; }
        public Action<string> Action;

        public CliShellCommand(string keyword, Action<string> action, string description = "")
        {
            this.Keyword = keyword;
            this.Description = description;
            this.Action = action;
        }

    }
}
