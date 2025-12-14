namespace Domain.Constants;

// Mensagens de erro centralizadas (usadas com string.Format)
public static class ErrorMessages
{
    public const string NotFound = "{0} não foi encontrado(a).";
    public const string AlreadyExists = "Já existe um registro de {0} com este {1}.";
    public const string RequiredField = "O campo {0} é obrigatório.";
    public const string InvalidOperation = "Operação inválida: {0}.";
    public const string CredentialsInvalid = "Usuário ou senha inválidos.";
}