using System;

namespace Gk;

[Flags]
public enum LHB_OPTION : uint
{
    NONE = 0U,
    VALIDATE_CHECKSUM = 1U,
    DEFAULT = 1U
}