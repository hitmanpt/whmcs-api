﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHMCS_API.ValidateLogin
{
    public class ValidateLogin
    {

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("userid")]
        public int UserID { get; set; }

        [JsonProperty("contactid")]
        public int ContactID { get; set; }

        [JsonProperty("passwordhash")]
        public string PasswordHash { get; set; }
    }
}
