using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsLSFront.Models
{
    public class LicenseSearchResults
    {
        public DbLicenseModel[] Licenses { get; set; }
        public int TotalCount { get; set; }
    }
}
