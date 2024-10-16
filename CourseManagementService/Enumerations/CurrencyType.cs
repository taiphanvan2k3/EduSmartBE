using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Enumerations
{
    public enum CurrencyType
    {
        [Comment("Viet Nam Dong")]
        VND = 1,

        [Comment("United States Dollar")]
        USD = 2
    }
}