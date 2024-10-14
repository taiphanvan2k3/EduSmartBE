namespace AuthService.Commons.Helpers
{
    public static class Utils
    {
        public static string GetDefaultAvatarUrl(string avatarUrl, string fullName, string username = "")
        {
            fullName = fullName.Trim();
            username = username.Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                if (string.IsNullOrEmpty(username))
                {
                    fullName = username;
                }
                else
                {
                    fullName = "Unknown Name";
                }
            }

            return string.IsNullOrEmpty(avatarUrl)
                ? $"https://ui-avatars.com/api/?name={fullName}&size=128&background=random"
                : avatarUrl;
        }
    }
}