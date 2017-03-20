using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Players.Interfaces;
using Players.Interfaces.Models;
using Users.Domain.Models;

namespace Players
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Players : Actor, IPlayersActor
    {
        /// <summary>
        /// Initializes a new instance of Players
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Players(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// Performs a geographic non-random movement for the user.
        /// </summary>
        /// <returns></returns>
        public async Task PerformMove()
        {
            var position = await GetPositionState();
            var user = await GetUserState();
            
            position.Latitude += 0.1;
            position.Longitude += 0.2;

            await PutPosition(position, CancellationToken.None);

            ActorEventSource.Current.ActorMessage(this, "Player {0} moved to: {1}.", user, position);
        }

        /// <summary>
        /// Get's the currenct geographical position of a player.
        /// </summary>
        /// <returns></returns>
        public async Task<Position> GetPositionState()
        {
            var position = await StateManager.TryGetStateAsync<Position>("position");
            return position.Value;
        }

        /// <summary>
        /// Get's the currenct geographical position of a player.
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetUserState()
        {
            var position = await StateManager.TryGetStateAsync<User>("user");
            return position.Value;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization
            return StateManager.TryAddStateAsync("position", new Position() { Latitude = 2.0, Longitude = 2.0 });
        }

        public Task PutPosition(Position position, CancellationToken cancellationToken)
        {
            return StateManager.AddOrUpdateStateAsync("position", position, (key, value) => { return position; });
        }

        public async Task PutUser(User user)
        {
            await StateManager.AddOrUpdateStateAsync("user", user, (key, value) => { return user; });
        }
    }
}
