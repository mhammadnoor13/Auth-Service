using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.UseCases
{
    public interface ICreateUserUseCase
    {
        Task<Guid> ExecuteAsync(Guid registrationId, string email, string password, CancellationToken ct);

    }
}
