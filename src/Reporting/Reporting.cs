using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Organizations.Domain;
using Players.Interfaces;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Users.Domain;

namespace Reporting
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Reporting : StatelessService, IReportingService
    {
        public Reporting(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => this.CreateServiceRemotingListener(context)) };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // Wait 20 seconds for services to start up.
            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

            // Run Seed once.
            await Seed();

            while (true)
            {
                var usersClient = ServiceProxy.Create<IUsersService>(new Uri("fabric:/WebDevKristiansand/Users"));

                var users = await usersClient.GetAll();

                foreach (var user in users)
                {
                    ActorId actorId = new ActorId(user.Id);

                    var serviceClient = ActorProxy.Create<IPlayersActor>(actorId, new Uri("fabric:/WebDevKristiansand/PlayersActorService"));

                    // Perform a move on all users every second.
                    await serviceClient.PerformMove();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task Seed()
        {
            var organizationsClient = ServiceProxy.Create<IOrganizationsService>(new Uri("fabric:/WebDevKristiansand/Organizations"));

            await organizationsClient.Put(new Organizations.Domain.Models.Organization() { Name = "Hooli" });
            await organizationsClient.Put(new Organizations.Domain.Models.Organization() { Name = "Pied Piper" });

            var usersClient = ServiceProxy.Create<IUsersService>(new Uri("fabric:/WebDevKristiansand/Users"));

            await usersClient.Put(new Users.Domain.Models.User() { FirstName = "Richard", LastName = "Hendricks", Id = new Guid("fcbd9a5c-339c-45dc-89e2-e095024d0a49") });
            await usersClient.Put(new Users.Domain.Models.User() { FirstName = "Dinesh", LastName = "Chugtai", Id = new Guid("a9b607e6-e27c-41ed-becb-c8ed848a3504") });
            await usersClient.Put(new Users.Domain.Models.User() { FirstName = "Bertram", LastName = "Gilfoyle", Id = new Guid("9ac00e69-b467-4585-8b47-b3b1715ad8e2") });
        }
    }
}
