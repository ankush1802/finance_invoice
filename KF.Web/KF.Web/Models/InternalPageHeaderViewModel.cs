using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KF.Web.Models
{
    public class InternalPageHeaderViewModel
    {
        public String HeaderFontAwsomeIcon { get; set; }
        public String HeaderTitle { get; set; }
        public String HeaderSubtitle { get; set; }
        public String HeaderParentPageName { get; set; }
        public String HeaderChildPageName { get; set; }
    }
}