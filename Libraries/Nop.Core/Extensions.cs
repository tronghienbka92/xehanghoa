

namespace Nop.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }
        public static string[] ThuTrongTuan = { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
        public static string toThu(this System.DateTime dt)
        {
            int i = (int)dt.DayOfWeek;
            return ThuTrongTuan[i];
        }
    }
}
