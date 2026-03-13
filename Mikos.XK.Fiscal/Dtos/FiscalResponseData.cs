using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Dtos
{
    public class FiscalResponseData
    {
        public string FiscalFolioNo { get; set; }
        public string FiscalBillGenerationDateTime { get; set; }
        public FiscalOutputs FiscalOutputs { get; set; }
        public StatusMessages StatusMessages { get; set; }
        public string chkGuid { get; set; }
    }

    public class FiscalOutputs
    {
        public List<Output> Output { get; set; }
    }

    public class Message
    {
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class Output
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class StatusMessages
    {
        public List<Message> Messages { get; set; }
    }
}
