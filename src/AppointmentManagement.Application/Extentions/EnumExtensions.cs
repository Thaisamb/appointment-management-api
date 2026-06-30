using System.ComponentModel;
using System.Reflection;

namespace AppointmentManagement.Application.Extensions;

public static class EnumExtensions
{
    public static string ToDescription(this Enum value)
    {
        return value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? value.ToString();
    }
}