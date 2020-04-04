﻿using RoystonGame.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoystonGame.Web.DataModels.Requests
{
    public class UserSubForm
    {
        [RegexSanitizer("^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$")]
        public Guid Id { get; set; }

        public string ShortAnswer { get; set; }

        [RegexSanitizer("^data:image\\/png;base64,[a-zA-Z0-9+/]+=*$")]
        public string Drawing { get; set; }

        public int? DropdownChoice { get; set; }
        public int? RadioAnswer { get; set; }

        [RegexSanitizer("^rgb\\([0-9]{1,3},[0-9]{1,3},[0-9]{1,3}\\)$")]
        public string Color { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Unmapped { get; set; }
    }
}