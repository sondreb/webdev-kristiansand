using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Models;

namespace Users.Domain
{
    public interface IUsersService : IService
    {
        Task<IEnumerable<User>> GetAll();

        Task<User> Put(User organization);
    }
}
