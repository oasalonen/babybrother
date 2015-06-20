using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Models
{
    public class IdModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is IdModel)
            {
                var other = obj as IdModel;
                if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(other.Id))
                {
                    return false;
                }
                else
                {
                    return Id == other.Id;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
