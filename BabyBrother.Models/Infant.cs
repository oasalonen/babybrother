using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Models
{
    public class Infant : IdModel
    {
        public enum GenderType
        {
            Male,
            Female,
            Other
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "birthdate")]
        public DateTimeOffset BirthDate { get; set; }
    }
}
