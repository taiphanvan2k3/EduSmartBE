namespace UserService.Commons.Helpers
{
    public class Utils
    {
        public static string GetGenderName(int gender)
        {
            return gender switch
            {
                1 => "Male",
                2 => "Female",
                _ => "Other"
            };
        }
    }
}