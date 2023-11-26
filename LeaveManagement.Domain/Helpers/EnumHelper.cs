using System.ComponentModel;
using System.Globalization;

namespace LeaveManagement.Domain.Helpers;

public static class EnumHelper
{
    public static string GetDescription<T>(this T e) where T : IConvertible
    {
        if(e is Enum)
        {
            Type type = e.GetType();
            Array values = Enum.GetValues(type);

            foreach(int value in values)
            {
                if(value == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memberInfo = type.GetMember(type.GetEnumName(value)!);

                    if (memberInfo[0]
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                    {
                        return descriptionAttribute.Description;
                    }
                }
            }
        }

        return string.Empty;
    }
}
