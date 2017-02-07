using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodleeBase.model
{
    public class FinappAuthenticationInfo
    {
        public string finappId { get; set; }
        public string token { get; set; }
    }

    public class SegmentInfo
    {
        public string segmentId { get; set; }
    }

    public class FastLinkAppToken
    {
        public List<FinappAuthenticationInfo> finappAuthenticationInfos { get; set; }
        public SegmentInfo segmentInfo { get; set; }
    }
}
