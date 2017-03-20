﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Organizations.Domain;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

using Users.Domain;
using Users.Domain.Models;
using Microsoft.ServiceFabric.Data;

namespace Users
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Users : StatefulService, IUsersService
    {
        private Task<IReliableDictionary<Guid, User>> Items => StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>("Users");

        public Users(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<IEnumerable<User>> GetAll()
        {
            var items = await Items;

            var result = new List<User>();

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var asyncEnumerable = await items.CreateEnumerableAsync(tx);

                using (var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator())
                {
                    while (await asyncEnumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(asyncEnumerator.Current.Value);
                    }
                }
            }

            return result;
        }

        public async Task<User> Put(User user)
        {
            var collection = await Items;

            using (var tx = this.StateManager.CreateTransaction())
            {
                await collection.AddOrUpdateAsync(tx, user.Id, user, (key, value) => { return user; });
                await tx.CommitAsync();
            }

            // Create or update Player Actor.

            return user;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }


        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //using (var tx = this.StateManager.CreateTransaction())
                //{
                //    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                //    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                //        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                //    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                //    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                //    // discarded, and nothing is saved to the secondary replicas.
                //    await tx.CommitAsync();
                //}

                var serviceClient = ServiceProxy.Create<IOrganizationsService>(new Uri("fabric:/WebDevKristiansand/Organizations"));
                var items = await serviceClient.GetAll();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Organizations Count: " + items.Count());

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
