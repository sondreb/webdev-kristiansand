using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Players.Interfaces.Models;
using Users.Domain.Models;

namespace Players.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IPlayersActor : IActor
    {
        Task PutPosition(Position position, CancellationToken cancellationToken);

        /// <summary>
        /// Performs a geographic non-random movement for the user.
        /// </summary>
        /// <returns></returns>
        Task PerformMove();

        Task PutUser(User user);

        /// <summary>
        /// Get's the currenct geographical position of a player.
        /// </summary>
        /// <returns></returns>
        Task<Position> GetPositionState();

        Task<User> GetUserState();
    }
}
