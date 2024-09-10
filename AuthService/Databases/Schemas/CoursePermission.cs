namespace AuthService.Databases.Schemas
{
    public class CoursePermission
    {
        public int CourseId { get; set; }

        public int AssistantId { get; set; }

        public string FunctionId { get; set; }

        public bool IsEnable { get; set; }

        public Function Function { get; set; }
    }
}