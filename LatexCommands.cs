using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToLateXAssistant
{
    public class LatexCommands
	{
        public IDictionary<string, string> actionToCommand;
        public LatexCommands()
        {
            actionToCommand = new Dictionary<string, string>
            {
                { "begin document", "\\begin{document}" },
                { "end document", "\\end{document}" },
                { "begin equation", "\\begin{equation}" },
                { "end equation", "\\end{equation}" },
                { "section", "\\section" },
                { "subsection", "\\subsection" },
                { "not equal to", "\\neq" },
                { "equals", "=" },
                { "equal to", "=" },
                { "less than", "<" },
                { "greater than", ">" },
                { "less than equal to", "<=" },
                { "greater than equal to", ">=" },
                { "foreach", "\\foreach" },
                { "therefore", "\\therefore" },
                { "lambda", "\\lambda" },
                { "new line", "\\\\" },
                { "next line", "\\\\" },
                { "log", "log" },
                { "sqrt", "\\sqrt" },
                { "plus", "+" },
                { "minus", "-" },
                { "multiply", "*" },
                { "divide", "/" },
                { "mod", "%" },
                { "+", "+" },
                { "-", "-" },
                { "*", "*" },
                { "/", "/" },
                { "%", "%" },
            };
        }
	}
}
