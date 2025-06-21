namespace Gateway.Services
{
    public static class InputValidator
    {
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            if (username.Length < 5 || username.Length > 20)
            {
                return false;
            }

            if (!username.All(c => char.IsLetter(c) || c == '_'))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (password.Length < 8 || password.Length > 32)
            {
                return false;
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
