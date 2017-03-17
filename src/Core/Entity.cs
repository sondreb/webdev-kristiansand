using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [DataContract]
    public class Entity : IExtensibleDataObject
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public DateTime Modified { get; set; }

        [DataMember]
        public Guid CreatedBy { get; set; }

        [DataMember]
        public Guid ModifiedBy { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
