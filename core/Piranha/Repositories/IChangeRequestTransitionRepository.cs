using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.Repositories
{
    /// <summary>
    /// Interface for change request transition repository.
    /// </summary>
    public interface IChangeRequestTransitionRepository
    {
        Task<IEnumerable<ChangeRequestTransition>> GetByChangeRequestIdAsync(Guid changeRequestId);
        Task SaveAsync(ChangeRequestTransition transition);
    }
}
