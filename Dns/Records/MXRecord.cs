using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns.Records
{

    #region RFC1035 Specification

    /*
3.3.9. MX RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                  PREFERENCE                   |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   EXCHANGE                    /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

PREFERENCE      A 16 bit integer which specifies the preference given to
                this RR among others at the same owner.  Lower values
                are preferred.

EXCHANGE        A <domain-name> which specifies a host willing to act as
                a mail exchange for the owner name.

MX records cause type A additional section processing for the host
specified by EXCHANGE.  The use of MX RRs is explained in detail in
[RFC-974].
*/

    #endregion

    public class MXRecord : ResourceRecord
    {

        #region Variables

        private short _preference;
        private string _exchange;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the preference given to
        /// this MX resource records among others at the same domain. Lower values
        /// are preferred.
        /// </summary>
        public short Preference
        {
            get { return _preference; }
            set { _preference = value; }
        }

        /// <summary>
        /// Gets / sets the host willing to act as
        /// a mail exchange for the domain name.
        /// </summary>
        public string Exchange
        {
            get { return _exchange; }
            set { _exchange = value; }
        }

        public override ResourceRecordType Type
        {
            get { return ResourceRecordType.MX; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the MXRecord class from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        public MXRecord(ResponseReader reader, string domainName)
            : base(reader, domainName)
        {
            _preference = reader.ReadInt16();
            _exchange = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10} {4,-8}, {5}", Name, TTL, Class, Type, _preference, _exchange);
        }

        /// <summary>
        /// Sorts the passed collection of MX records based on their preference numbers. Sorted for descending preferency.
        /// Equally preferenced servers relative positions are randomised in order to make load balancing work correctly.
        /// </summary>
        /// <param name="mxRecords"></param>
        /// <returns></returns>
        public static List<MXRecord> Sort(IEnumerable<MXRecord> mxRecords)
        {

            // Declare variables
            List<MXRecord> sortedList;

            sortedList = new List<MXRecord>();
            foreach (MXRecord mxRecord in mxRecords)
            {
                if (sortedList.Count == 0)
                {
                    sortedList.Add(mxRecord);
                    continue;
                }
                for (int i = 0; i < sortedList.Count; i++)
                {
                    if (mxRecord._preference > sortedList[i]._preference)
                        continue;
                    sortedList.Insert(i, mxRecord);
                    break;
                }
            }

            return sortedList;
        }

        #endregion
    }
}
