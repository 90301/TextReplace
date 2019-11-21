using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableText.LogProcessor
{
    interface ProgramNodeInterface
    {
        String calculate(String input);
        String getOpName();
        string createExample();
    }
}
