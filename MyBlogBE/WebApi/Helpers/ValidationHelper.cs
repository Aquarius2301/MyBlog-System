using System.Text.RegularExpressions;

namespace WebApi.Helpers;

public class ValidationHelper
{
    /// <summary>
    /// Validates if the provided password meets strength requirements.
    /// </summary>
    /// <param name="password">The password string to validate.</param>
    /// <returns><c>true</c> if the password is strong; otherwise, <c>false</c>.</returns>
    public static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Regex: (?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$";
        return Regex.IsMatch(password, pattern);
    }

    /// <summary>
    /// Validates if the provided string is in a valid email format.
    /// </summary>
    /// <param name="email">The email string to validate.</param>
    /// <returns><c>true</c> if the email is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates if the provided string meets specified criteria.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <param name="onlyAlphaNumeric">
    /// If <c>true</c>, the generated string contains only letters and digits.
    /// If <c>false</c>, the string is Base64-encoded, which may include symbols.
    /// </param>
    /// <param name="minLength">Minimum length of the string.</param>
    /// <param name="maxLength">Maximum length of the string.</param>
    /// <returns></returns>
    public static bool IsValidString(
        string input,
        bool onlyAlphaNumeric = false,
        int minLength = 1,
        int maxLength = 1000
    )
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.Length < minLength || input.Length > maxLength)
            return false;

        if (onlyAlphaNumeric)
        {
            string pattern = @"^[a-zA-Z0-9]*$";
            return Regex.IsMatch(input, pattern);
        }

        return true;
    }

    /// <summary>
    /// Validates if the provided date of birth indicates an age of at least 13 years.
    /// </summary>
    /// <param name="dob">The date of birth to validate.</param>
    /// <returns><c>true</c> if the date of birth is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidDateOfBirth(DateTime dob)
    {
        var today = DateTime.Today;
        int age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
            age--;

        return dob <= today && age >= 13;
    }
}
