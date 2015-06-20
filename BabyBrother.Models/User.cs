﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabyBrother.Models
{
    public class User : IdModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
