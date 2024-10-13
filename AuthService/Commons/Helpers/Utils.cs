namespace AuthService.Commons.Helpers
{
    public static class Utils
    {
        public static string GetDefaultAvatarUrl(string avatarUrl, string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = "Unknown name";
            }
            
            return string.IsNullOrEmpty(avatarUrl)
                ? $"https://ui-avatars.com/api/?name={fullName}&size=128&background=random"
                : avatarUrl;
        }
    }
}