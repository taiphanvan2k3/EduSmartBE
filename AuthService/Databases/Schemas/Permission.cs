namespace AuthService.Databases.Schemas
{
    public class Permission
    {
        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        public Function Function { get; set; }
    }
}