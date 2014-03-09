using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemliCMS.Models
{
    public class TitleModel
    {
        public string Name { get; set; }

        public string TranslationId { get; set; }

        public string Title { get; set; }

        public bool isActive { get; set; }
    }
}