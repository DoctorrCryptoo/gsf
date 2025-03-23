﻿//******************************************************************************************************
//  TimeTag.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/03/2006 - J. Ritchie Carroll
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, GSF.
//  07/12/2006 - J. Ritchie Carroll
//       Modified class to be derived from new "TimeTagBase" class.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/02/2009 - Pinal C. Patel
//       Added Parse() static method to allow conversion of string to TimeTag.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/25/2009 - Pinal C. Patel
//       Added overloaded constructor that take ticks.
//       Added Now and UtcNow static properties for ease-of-use.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace GSF.Historian;

/// <summary>
/// Represents a historian time tag as number of seconds from the <see cref="BaseDate"/>.
/// </summary>
[Serializable]
public class TimeTag : TimeTagBase, IComparable<TimeTag>
{
    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeTag"/> class.
    /// </summary>
    /// <param name="ticks">Number of ticks since the <see cref="BaseDate"/>.</param>
    public TimeTag(long ticks)
        : base(BaseDate.Ticks, ticks)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeTag"/> class.
    /// </summary>
    /// <param name="seconds">Number of seconds since the <see cref="BaseDate"/>.</param>
    public TimeTag(decimal seconds)
        : base(BaseDate.Ticks, seconds)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeTag"/> class.
    /// </summary>
    /// <param name="timestamp"><see cref="DateTime"/> value on or past the <see cref="BaseDate"/>.</param>
    public TimeTag(DateTime timestamp)
        : base(BaseDate.Ticks, timestamp)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TimeTag"/> from serialization parameters.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> with populated with data.</param>
    /// <param name="context">The source <see cref="StreamingContext"/> for this deserialization.</param>
    protected TimeTag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Returns the default text representation of <see cref="TimeTag"/>.
    /// </summary>
    /// <returns><see cref="string"/> that represents <see cref="TimeTag"/>.</returns>
    public override string ToString()
    {
        return ToString("dd-MMM-yyyy HH:mm:ss.fff");
    }

    /// <summary>
    /// Compares this time tag instance to another time tag instance and returns an integer that indicates whether the value of this instance
    /// is less than, equal to, or greater than the value of the other time tag.
    /// </summary>
    /// <param name="other">A <see cref="TimeTag"/> instance to compare.</param>
    /// <returns>A signed number indicating the relative values of this instance and the other value.</returns>
    /// <remarks>
    /// Time tags are compared by their value.
    /// </remarks>
    public int CompareTo(TimeTag other)
    {
        decimal tVal = Value;
        decimal oVal = other.Value;
        return tVal < oVal ? -1 : tVal > oVal ? 1 : 0;
    }

    #endregion

    #region [ Static ]

    // Static Fields

    /// <summary>
    /// Represents the smallest possible value of <see cref="TimeTag"/>.
    /// </summary>
    public static readonly TimeTag MinValue;

    /// <summary>
    /// Represents the largest possible value of <see cref="TimeTag"/>.
    /// </summary>
    public static readonly TimeTag MaxValue;

    /// <summary>
    /// Represents the base <see cref="DateTime"/> (01/01/1995) for <see cref="TimeTag"/>.
    /// </summary>
    public static readonly DateTime BaseDate;

    // Static Constructor

    static TimeTag()
    {
        BaseDate = new DateTime(1995, 1, 1, 0, 0, 0);
        MinValue = new TimeTag(0.0M);
        MaxValue = new TimeTag(2147483647.999M);
    }

    // Static Properties

    /// <summary>
    /// Gets a <see cref="TimeTag"/> object that is set to the current date and time on this computer, expressed as the local time.
    /// </summary>
    public static TimeTag Now => new(DateTime.Now.Ticks - BaseDate.Ticks);

    /// <summary>
    /// Gets a <see cref="TimeTag"/> object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static TimeTag UtcNow => new(DateTime.UtcNow.Ticks - BaseDate.Ticks);

    // Static Methods

    /// <summary>
    /// Converts the specified string representation of a date and time to its <see cref="TimeTag"/> equivalent.
    /// </summary>
    /// <param name="timetag">A string containing the date and time to convert.</param>
    /// <returns>A <see cref="TimeTag"/> object.</returns>
    /// <remarks>
    /// <paramref name="timetag"/> can be specified in one of the following format:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Time Format</term>
    ///         <description>Format Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>12-30-2000 23:59:59</term>
    ///         <description>Absolute date and time.</description>
    ///     </item>
    ///     <item>
    ///         <term>*</term>
    ///         <description>Evaluates to <see cref="DateTime.UtcNow"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term>*-20s</term>
    ///         <description>Evaluates to 20 seconds before <see cref="DateTime.UtcNow"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term>*-10m</term>
    ///         <description>Evaluates to 10 minutes before <see cref="DateTime.UtcNow"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term>*-1h</term>
    ///         <description>Evaluates to 1 hour before <see cref="DateTime.UtcNow"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term>*-1d</term>
    ///         <description>Evaluates to 1 day before <see cref="DateTime.UtcNow"/>.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static TimeTag Parse(string timetag)
    {
        DateTime dateTime;
        timetag = timetag.ToLower();

        if (timetag.Contains("*"))
        {
            // Relative time is specified.
            // Examples:
            // 1) * (Now)
            // 2) *-20s (20 seconds ago)
            // 3) *-10m (10 minutes ago)
            // 4) *-1h (1 hour ago)
            // 5) *-1d (1 day ago)
            dateTime = DateTime.UtcNow;

            if (timetag.Length <= 1)
                return new TimeTag(dateTime);
            
            string unit = timetag.Substring(timetag.Length - 1);
            int adjustment = int.Parse(timetag.Substring(1, timetag.Length - 2));

            dateTime = unit switch
            {
                "s" => dateTime.AddSeconds(adjustment),
                "m" => dateTime.AddMinutes(adjustment),
                "h" => dateTime.AddHours(adjustment),
                "d" => dateTime.AddDays(adjustment),
                _ => dateTime
            };
        }
        else
        {
            // Absolute time is specified.
            dateTime = DateTime.Parse(timetag, CultureInfo.InvariantCulture);
        }

        return new TimeTag(dateTime);
    }

    #endregion
}