﻿using Lucene.Net.Support;
using System;
using System.Globalization;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Config
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// LUCENENET specific enum for mimicking the Java DateFormat
    /// </summary>
    public enum DateFormat
    {
        FULL, LONG,
        MEDIUM, SHORT
    }

    /// <summary>
    /// This <see cref="NumberFormat"/> parses <see cref="long"/> into date strings and vice-versa. It
    /// uses the given <paramref name="dateFormat"/> and <see cref="CultureInfo">locale</see> to parse and format dates, but before, it
    /// converts <see cref="long"/> to <see cref="DateTime"/> objects or vice-versa.
    /// <para/>
    /// Note that the <see cref="long"/> value the dates are parsed into and out of represent the number of milliseconds
    /// since January 1, 1970 0:00:00, also known as the "epoch".
    /// </summary>
    public class NumberDateFormat : NumberFormat
    {
        //private static readonly long serialVersionUID = 964823936071308283L;

        // The .NET ticks representing January 1, 1970 0:00:00, also known as the "epoch".
        public const long EPOCH = 621355968000000000;

        private readonly string dateFormat;
        private readonly DateFormat dateStyle;
        private readonly DateFormat timeStyle;
        private TimeZoneInfo timeZone = TimeZoneInfo.Local;

        /// <summary>
        /// Constructs a <see cref="NumberDateFormat"/> object using the given <paramref name="dateFormat"/>
        /// and <paramref name="locale"/>.
        /// </summary>
        /// <param name="dateFormat">Date format used to parse and format dates</param>
        /// <param name="locale"></param>
        public NumberDateFormat(string dateFormat, CultureInfo locale)
            : base(locale)
        {
            this.dateFormat = dateFormat;
        }

        /// <summary>
        /// Constructs a <see cref="NumberDateFormat"/> object using the given <paramref name="dateStyle"/>,
        /// <paramref name="timeStyle"/>, and <paramref name="locale"/>.
        /// </summary>
        public NumberDateFormat(DateFormat dateStyle, DateFormat timeStyle, CultureInfo locale)
            : base(locale)
        {
            this.dateStyle = dateStyle;
            this.timeStyle = timeStyle;
        }

        public virtual TimeZoneInfo TimeZone
        {
            get { return this.timeZone; }
            set { this.timeZone = value; }
        }

        public override string Format(double number)
        {
            return new DateTime(EPOCH).AddMilliseconds(number).ToString(GetDateFormat(), this.locale);
        }

        public override string Format(long number)
        {
            return new DateTime(EPOCH).AddMilliseconds(number).ToString(GetDateFormat(), this.locale);
        }

        public override object Parse(string source)
        {
            return (DateTime.Parse(source, this.locale) - new DateTime(EPOCH)).TotalMilliseconds;
        }

        public override string Format(object number)
        {
            return new DateTime(EPOCH).AddMilliseconds(Convert.ToInt64(number)).ToString(GetDateFormat(), this.locale);
        }

        /// <summary>
        /// Returns the .NET date format that will be used to Format the date.
        /// Note that parsing the date uses <see cref="DateTime.Parse(string, IFormatProvider)"/>, which
        /// does not require a format.
        /// </summary>
        // LUCENENET specific
        public string GetDateFormat()
        {
            if (dateFormat != null) return dateFormat;

            string datePattern = "", timePattern = "";

            switch (dateStyle)
            {
                case DateFormat.SHORT:
                    datePattern = locale.DateTimeFormat.ShortDatePattern;
                    break;
                case DateFormat.MEDIUM:
                    datePattern = locale.DateTimeFormat.LongDatePattern
                        .Replace("dddd,", "").Replace(", dddd", "") // Remove the day of the week
                        .Replace("MMMM", "MMM"); // Replace month with abbreviated month
                    break;
                case DateFormat.LONG:
                    datePattern = locale.DateTimeFormat.LongDatePattern
                        .Replace("dddd,", "").Replace(", dddd", ""); // Remove the day of the week
                    break;
                case DateFormat.FULL:
                    datePattern = locale.DateTimeFormat.LongDatePattern;
                    break;
            }

            switch (timeStyle)
            {
                case DateFormat.SHORT:
                    timePattern = locale.DateTimeFormat.ShortTimePattern;
                    break;
                case DateFormat.MEDIUM:
                    timePattern = locale.DateTimeFormat.LongTimePattern;
                    break;
                case DateFormat.LONG:
                    timePattern = locale.DateTimeFormat.LongTimePattern; // LUCENENET TODO: Time zone info not being added
                    break;
                case DateFormat.FULL:
                    timePattern = locale.DateTimeFormat.LongTimePattern; // LUCENENET TODO: Time zone info not being added, but Java doc is unclear on what the difference is between this and LONG
                    break;
            }

            return string.Concat(datePattern, " ", timePattern);
        }
    }
}
