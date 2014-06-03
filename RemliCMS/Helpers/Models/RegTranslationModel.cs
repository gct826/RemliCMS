using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.Models
{
    public class RegTranslationModel
    {
        public int Value { get; set; }

        public string translationId { get; set; }

        public string text { get; set; }
    }
}