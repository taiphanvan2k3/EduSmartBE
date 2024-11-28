namespace PaymentService.Enumerations
{
    public enum TransactionType
    {
        BuyCourse = 1, // Sinh viên mua khoá học
        ExtendStorage = 2, // Giảng viên mua thêm dung lượng lưu trữ
        UpgradeCourse = 3, // Giảng viên nâng cấp các tính năng của khoá học
        DrawingRequest = 4 // Giảng viên yêu cầu rút tiền
    }
}