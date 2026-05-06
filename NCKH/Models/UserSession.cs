namespace NCKH.Models
{
    public static class UserSession
    {
        // Giả lập thông tin user. Sau này bạn sẽ gán giá trị này khi Login thành công.
        public static string Role { get; set; } = "teacher"; // Mặc định để test là teacher

        // Kiểm tra xem có phải giảng viên không
        public static bool IsTeacher => Role.ToLower() == "teacher";
    }
}