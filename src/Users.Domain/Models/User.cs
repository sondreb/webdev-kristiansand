using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Models
{
    [DataContract(Name = "organization", Namespace = "http://schemas.webdevkristiansand/2017/03/organization")]
    public class User : Entity
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }
}
