using System.ComponentModel;

namespace iambetter.Domain.Enums
{

    public enum MatchStatus
    {
        [Description("Unknown")]
        Unknown,
        [Description("Not Started")]
        NS,
        [Description("Match Finished")]
        FT,
    }
}

