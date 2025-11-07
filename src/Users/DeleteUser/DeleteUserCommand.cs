namespace Sphera.API.Users.DeleteUser;

/// <summary>
/// Representa o comando para deletar um usuário identificado por seu Id.
/// </summary>
/// <param name="Id">Identificador único do usuário.</param>
public record DeleteUserCommand(Guid Id);