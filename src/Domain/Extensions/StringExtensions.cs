namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para manipulação de strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Capitaliza a primeira letra da string e converte o restante para minúsculo.
    /// </summary>
    /// <param name="str">String a ser capitalizada.</param>
    /// <returns>String com a primeira letra maiúscula e o restante minúsculo.</returns>
    /// <example>
    /// "gustavo" -> "Gustavo"
    /// "GUSTAVO" -> "Gustavo"
    /// "gUsTaVo" -> "Gustavo"
    /// </example>
    public static string Capitalize(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;

        return char.ToUpper(str[0]) + str[1..].ToLower();
    }
}
