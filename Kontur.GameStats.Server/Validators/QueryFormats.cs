using System;
using System.Globalization;
using Nancy.Routing.Constraints;

namespace Kontur.GameStats.Server.Validators
{
    public static class QueryFormats
    {
        public static readonly string Endpoint = "{ipOrHostname}-{port:range(0,65535)}";
        public static readonly string Timestamp = "{timestamp:timestamp}";
        public static readonly string PlayerName = "{name}";
        public static readonly string Count = "{count?5}";
        public static readonly string DatetimeFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        public class TimestampConstraint : RouteSegmentConstraintBase<DateTime>
        {
            public override string Name => "timestamp";

            protected override bool TryMatch(string constraint, string segment, out DateTime matchedValue)
            {
                var culture = CultureInfo.InvariantCulture;
                const DateTimeStyles dtStyle = DateTimeStyles.None;
                return DateTime.TryParseExact(segment, DatetimeFormat, culture, dtStyle, out matchedValue);
            }
        }
    }
}
