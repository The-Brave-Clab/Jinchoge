using System;

namespace Gk;

public static class EnumExt
{
    public static bool HasFlag<T>(T self, T flag) where T : IConvertible
    {
        if (self.GetType() != flag.GetType())
        {
            return false;
        }

        var num = Convert.ToUInt64(self);
        var num2 = Convert.ToUInt64(flag);
        return (num & num2) == num2;
    }
}