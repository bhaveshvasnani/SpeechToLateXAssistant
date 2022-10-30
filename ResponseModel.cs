using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToTextAssistant
{
    public class ResponseModel
    {
        public string query { get; set; }
        public CustomPrediction prediction { get; set; }
    }

    public class CustomPrediction
    {
        public string topIntent { get; set; }
        public CustomEntity entities { get; set; }

    }

    public class CustomEntity
    {
        public IList<Variables> Variable { get; set; }
        public IList<string> Operator { get; set; }
    }

    public class Variables
    {
        public IList<string> Variable1 { get; set; }
        public IList<string> Variable2 { get; set; }
        public IList<string> Variable3 { get; set; }
    }
}
