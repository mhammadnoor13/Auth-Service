using MassTransit;
using Contracts.Commands;
using Contracts.Responses;
using AuthService.Application.UseCases;

namespace AuthService.Api.Consumers
{
    public class CreateUserConsumer : IConsumer<CreateUserCommand>
    {
        private readonly ICreateUserUseCase _createUser;

        public CreateUserConsumer(ICreateUserUseCase createUser)
        {
            _createUser = createUser;
        }


        public async Task Consume(ConsumeContext<CreateUserCommand> context)
        {
            try
            {
                var cmd = context.Message;
                var userId = await _createUser.ExecuteAsync(
                    cmd.RegistrationId,
                    cmd.Email,
                    cmd.Password,
                    context.CancellationToken);

                await context.RespondAsync(new UserCreatedResponse(userId));
            }
            catch (InvalidOperationException ex)
            {
                // MassTransit will automatically publish a Fault<CreateUserCommand>
                throw;
            }
        }


    }
}
