using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace asp.netwebmvcWebsite.Models
{
    public class NameModel
    {
       [Required]
        public string name { get; set; }
    }
}