using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.UseCases
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashProvider _passwordHashProvider;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserUseCase(IUserRepository userRepository, IPasswordHashProvider passwordHashProvider, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHashProvider = passwordHashProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> ExecuteAsync(Guid registrationId, string email, string password, CancellationToken ct)
        {
            if (await _userRepository.ExistsAsync(email,ct))
            {
                throw new InvalidOperationException("Email is already registered.");
            }


            var user = User.Create(email);
            user.SetPasswordHash(_passwordHashProvider.Hash(password));

            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.CommitAsync(ct);

            return user.Id;

        }
    }
}
