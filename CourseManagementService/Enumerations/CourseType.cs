using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Enumerations
{
    public enum CourseType
    {
        [Comment("A course that is a collection of videos")]
        Tutorial,

        [Comment("A course that is live and interactive")]
        Direct
    }
}