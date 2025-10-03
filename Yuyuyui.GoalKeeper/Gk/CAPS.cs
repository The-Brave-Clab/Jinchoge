using System;

namespace Gk;

[Flags]
public enum CAPS : uint
{
    TAKESHI_ENC = 2147483648U,
    TAKESHI_DEC = 1073741824U,
    IGARASHI = 536870912U,
    CAPS_COPY = 1U,
    REFERENCE = 2U,
    HAVE_IV = 4U
}