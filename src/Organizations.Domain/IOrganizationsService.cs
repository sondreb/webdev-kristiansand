using Microsoft.ServiceFabric.Services.Remoting;
using Organizations.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organizations.Domain
{
    public interface IOrganizationsService : IService
    {
        Task<IEnumerable<Organization>> GetAll();

        Task<Organization> Put(Organization organization);

        Task<Organization> Delete(Guid id);
    }
}
