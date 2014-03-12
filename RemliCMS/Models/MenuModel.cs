using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemliCMS.Models
{
    public class MenuModel
    {
        public string Permalink { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        public bool IsCurrent { get; set; }
    }
}