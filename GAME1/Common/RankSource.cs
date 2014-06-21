using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GAME1.Common
{
    //[DataContract]
    public class RankSource
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }


        [JsonProperty(PropertyName = "Score")]
        public int Score { get; set; }


        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        //public RankSource() { }
       
    }
}
