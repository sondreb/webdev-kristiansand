﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Organizations.Domain.Models;
using Organizations.Domain;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Organizations
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Organizations : StatefulService, IOrganizationsService
    {
        private Task<IReliableDictionary<Guid, Organization>> Items => StateManager.GetOrAddAsync<IReliableDictionary<Guid, Organization>>("Organizations");

        public Organizations(StatefulServiceContext context)
            : base(context)
        { }

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
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var items = await Items;

                    var organization = new Organization()
                    {
                        Name = "Hooli",
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        CreatedBy = Guid.NewGuid()
                    };

                    await items.AddAsync(tx, Guid.NewGuid(), organization);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    //await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public async Task<IEnumerable<Organization>> GetAll()
        {
            var items = await Items;

            var result = new List<Organization>();

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var asyncEnumerable = await items.CreateEnumerableAsync(tx);

                using (var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator())
                {
                    while (await asyncEnumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(asyncEnumerator.Current.Value);

                        // Here you could implement a filter.
                        //if (filter(asyncEnumerator.Current.Value))
                        //{
                        //    result.Add(asyncEnumerator.Current.Value);
                        //}
                    }
                }
            }

            return result;
        }

        public async Task<Organization> Put(Organization organization)
        {
            var collection = await Items;

            using (var tx = this.StateManager.CreateTransaction())
            {
                await collection.AddOrUpdateAsync(tx, organization.Id, organization, (key, value) => { return organization; });
                await tx.CommitAsync();
            }

            return organization;
        }

        public Task<Organization> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
