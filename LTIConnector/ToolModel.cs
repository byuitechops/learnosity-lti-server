using LtiLibrary.Lti1;

namespace Byui.LTIConnector.Models
{
    public class ToolModel
    {
        public string ConsumerSecret { get; set; }
        public LtiRequest LtiRequest { get; set; }
    }
}