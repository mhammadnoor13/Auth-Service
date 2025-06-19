namespace Contracts;

public interface IUserRegistered
{
    Guid Id { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    string Speciality { get; }
}
