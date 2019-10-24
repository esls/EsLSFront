using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EsLSFront.Models
{
    public class PaginatedLicensesModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<DbLicenseModel> Data { get; set; }
        public string MailFiter { get; set; }
        public string KeyFilter { get; set; }
    }
}
