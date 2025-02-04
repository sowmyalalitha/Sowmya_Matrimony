using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace asp.netwebmvcWebsite.Models
{
    public class PhotoViewModel
    {
        public string RegisteredEmailId { get; set; }

        public byte[]  MainImage1 { get; set; }
        public string Name { get; set; }
        public string base64Image { get; set; }

        public string MainImage1FilePath { get; set; }
    }
}