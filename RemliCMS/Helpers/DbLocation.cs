using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemliCMS.Helpers
{
    public class DbLocation
    {
        public string GetDbLocation()
        {
            string connectionString = HttpContext.Current.Application["MongoDbLocation"] as string;

            return connectionString;
        }
    }
}