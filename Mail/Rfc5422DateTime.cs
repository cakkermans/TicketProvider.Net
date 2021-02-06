using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The Rfc5422DateTime class represents an RFC5422 formatted timestamp.
    /// </summary>
    public class Rfc5422DateTime
    {

        private DateTime _dateTime;
		private TimeZone _timeZone;

        /// <summary>
        /// Initializes a new instance of the 
        /// </summary>
        public Rfc5422DateTime()
        {
        }

		/// <summary>
		/// Create an instance of the RFC 2822 date object.
		/// </summary>
		/// <param name="datetime">The date to convert</param>
		/// <param name="timezone">The timezone to use in the conversion</param>
        public Rfc5422DateTime(DateTime dateTime, TimeZone timeZone)
		{
            _dateTime = dateTime;
            _timeZone = timeZone;
		}
		
		/// <summary>
		/// Generate the date string
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
		{

            TimeZone currentTz;
            TimeSpan currentTzOffset;
            string timeZone;

            currentTz = TimeZone.CurrentTimeZone;
            currentTzOffset = currentTz.GetUtcOffset(_dateTime);
            if(currentTzOffset.Hours >= 0)
                timeZone = string.Format("+{0:00}{1:00}", currentTzOffset.Hours, Math.Abs(currentTzOffset.Minutes % 60));
            else
                timeZone = string.Format("{0:00}{1:00}", currentTzOffset.Hours, Math.Abs(currentTzOffset.Minutes % 60));

			//return _datetime.ToString("ddd, d MMM yyyy HH:mm:ss "+tz, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                "{0:ddd, d MMM yyyy HH:mm:ss} {1}", _dateTime, timeZone );
		}
    }
}
