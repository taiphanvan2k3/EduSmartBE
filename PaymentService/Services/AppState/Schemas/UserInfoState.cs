namespace PaymentService.Services.AppState.Schemas
{
    public class UserInfoState
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}