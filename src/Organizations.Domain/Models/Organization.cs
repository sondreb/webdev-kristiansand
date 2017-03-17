using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Organizations.Domain.Models
{
    [DataContract(Name = "organization", Namespace = "http://schemas.webdevkristiansand/2017/03/organization")]
    public class Organization : Entity
    {
        [DataMember]
        public string Name { get; set; }
    }
}
