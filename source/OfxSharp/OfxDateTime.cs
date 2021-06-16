using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OfxSharp
{
    public static class OfxDateTime
    {
        private const RegexOptions _o = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        // The format of `datetime` values is described in section 3.2.8.2:
        private static readonly Regex _dateFmt        = new Regex( @"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)$"                                                                             , _o ); // Assume noon in UTC.
        private static readonly Regex _dateTimeFmt    = new Regex( @"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hours>\d\d)(?<minutes>\d\d)(?<seconds>\d\d)$"                               , _o ); // Assume UTC.
        private static readonly Regex _dateTimeMSFmt  = new Regex( @"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hours>\d\d)(?<minutes>\d\d)(?<seconds>\d\d)\.(?<ms>\d\d\d)$"                , _o ); // Assume UTC.
        private static readonly Regex _dateTimeMSZFmt = new Regex( @"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hours>\d\d)(?<minutes>\d\d)(?<seconds>\d\d)\.(?<ms>\d\d\d)\[(?<zone>.+?)\]$", _o ); // Zone is specified.
        private static readonly Regex _dateTimeZFmt   = new Regex( @"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hours>\d\d)(?<minutes>\d\d)(?<seconds>\d\d)\[(?<zone>.+?)\]$"               , _o ); // Zone is specified.
    
        private static readonly Regex _zoneFmt        = new Regex( @"^(?<offset>(?<hours>(?:\-|\+)?\d+)(?:\.(?<minutes>\d\d))?)(?<name>:\w+)?$", _o ); // `[gmt offset:tz name]`, e.g. "[-5:EST]", "[-5.50:EST]", "[+12.12:WTF]", etc. The GMT Offset may be fractional. The OFX spec requires the zone name, but as it's not useful I see no harm in making the zone-name part optional.

        private static Int32 ParseInt32( String text ) => Int32.Parse( text, NumberStyles.Integer, CultureInfo.InvariantCulture );

        /// <summary>Returns null if <paramref name="s"/> cannot be parsed as an OFX date or datetime.</summary>
        public static DateTimeOffset? MaybeParseOfxDateTime( this String s )
        {
            if( TryParseOfxDateTime( s, out DateTimeOffset value, errorMessage: out _ ) )
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Throws a <see cref="FormatException"/> if <paramref name="s"/> cannot be parsed as an OFX date or datetime.</summary>
        public static DateTimeOffset RequireParseOfxDateTime( this String s )
        {
            if( TryParseOfxDateTime( s, out DateTimeOffset value, out String errorMessage ) )
            {
                return value;
            }
            else
            {
                throw new FormatException( message: errorMessage );
            }
        }

        public static Boolean TryParseOfxDateTime( String s, out DateTimeOffset value, out String errorMessage )
        {
            if( String.IsNullOrWhiteSpace( s ) )
            {
                value        = default;
                errorMessage = "Value is null or empty";
                return false;
            }

            s = s.Trim();

            // Test each regex, in most-to-least-specific-order:

            Match m = _dateTimeMSZFmt.Match( s );
            if( m.Success )
            {
                if( !TryParseOffset( zoneGroup: m.Groups["zone"].Value, offset: out TimeSpan offset, zoneName: out _, errorMessage: out errorMessage ) )
                {
                    value = default;
                    return false;
                }
                else
                {
                    return TryCreateDateTimeOffset( m, offset, out value, out errorMessage );
                }
            }
        
            m = _dateTimeMSFmt.Match( s );
            if( m.Success )
            {
                return TryCreateDateTimeOffset( m, offset: TimeSpan.Zero, out value, out errorMessage );
            }
        
            m = _dateTimeFmt.Match( s );
            if( m.Success )
            {
                return TryCreateDateTimeOffset( m, offset: TimeSpan.Zero, out value, out errorMessage );
            }
        
            m = _dateFmt.Match( s );
            if( m.Success )
            {
                return TryCreateDateTimeOffset( m, offset: TimeSpan.Zero, out value, out errorMessage );
            }
        
            m = _dateTimeZFmt.Match( s );
            if( m.Success )
            {
                if( !TryParseOffset( zoneGroup: m.Groups["zone"].Value, offset: out TimeSpan offset, zoneName: out _, errorMessage: out errorMessage ) )
                {
                    value = default;
                    return false;
                }
                else
                {
                    return TryCreateDateTimeOffset( m, offset, out value, out errorMessage );
                }
            }
            
            value        = default;
            errorMessage = "Unrecognized date-time format: \"" + s + "\"";
            return false;
        }

        private static Boolean TryParseOffset( String zoneGroup, out TimeSpan offset, out String zoneName, out String errorMessage )
        {
            Match zm = _zoneFmt.Match( zoneGroup );
            if( !zm.Success )
            {
                offset       = default;
                zoneName     = default;
                errorMessage = "Couldn't regex-match the time-zone and GMT-offset text: \"{0}\".".Fmt( zoneGroup );
                return false;
            }
            else
            {
                zoneName = zm.Groups["name"].Value;
                return TryParseOffset( zoneFmtMatch: zm, out offset, out errorMessage );
            }
        }

        private static Boolean TryParseOffset( Match zoneFmtMatch, out TimeSpan offset, out String errorMessage )
        {
            String zoneOffsetHoursStr   = zoneFmtMatch.Groups["hours"]  ?.Value ?? String.Empty;
            String zoneOffsetMinutesStr = zoneFmtMatch.Groups["minutes"]?.Value ?? String.Empty;

            if( Int32.TryParse( zoneOffsetHoursStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 hours ) )
            {
                TimeSpan hoursSpan = TimeSpan.FromHours(hours);

                if( zoneOffsetMinutesStr.IsEmpty() )
                {
                    offset       = hoursSpan;
                    errorMessage = null;
                    return true;
                }
                else if( Int32.TryParse( zoneOffsetMinutesStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 mins ) )
                {
                    TimeSpan minutesSpan = TimeSpan.FromMinutes(mins); // <-- This is always an unsigned absolute value (magnitude).

                    if( hours < 0 )
                    {
                        offset       = hoursSpan.Subtract( minutesSpan );
                        errorMessage = null;
                        return true;
                    }
                    else
                    {
                        offset       = hoursSpan.Add( minutesSpan );
                        errorMessage = null;
                        return true;
                    }
                }
                else
                {
                    offset       = default;
                    errorMessage = "Couldn't parse \"{0}\" as GMT offset minutes.".Fmt( zoneOffsetMinutesStr );
                    return true;
                }
            }
            else
            {
                offset       = default;
                errorMessage = "Couldn't parse \"{0}\" as GMT offset hours.".Fmt( zoneOffsetHoursStr );
                return true;
            }
        }

        private static Boolean TryCreateDateTimeOffset( Match m, TimeSpan offset, out DateTimeOffset value, out String errorMessage )
        {
            try
            {
                value = new DateTimeOffset(
                    year       : m.Groups["year"   ] is Group yearGrp    && yearGrp   .Success ? ParseInt32( yearGrp   .Value ) : 0,
                    month      : m.Groups["month"  ] is Group monthGrp   && monthGrp  .Success ? ParseInt32( monthGrp  .Value ) : 0,
                    day        : m.Groups["day"    ] is Group dayGrp     && dayGrp    .Success ? ParseInt32( dayGrp    .Value ) : 0,
                
                    hour       : m.Groups["hours"  ] is Group hoursGrp   && hoursGrp  .Success ? ParseInt32( hoursGrp  .Value ) : 0,
                    minute     : m.Groups["minutes"] is Group minutesGrp && minutesGrp.Success ? ParseInt32( minutesGrp.Value ) : 0,
                    second     : m.Groups["seconds"] is Group secondsGrp && secondsGrp.Success ? ParseInt32( secondsGrp.Value ) : 0,
                    millisecond: m.Groups["ms"     ] is Group msGrp      && msGrp     .Success ? ParseInt32( msGrp     .Value ) : 0,
                    offset     : offset
                );

                errorMessage = null;
                return true;
            }
            catch( ArgumentException argEx )
            {
                value        = default;
                errorMessage = argEx.Message;
                return false;
            }
        }
    }
}
