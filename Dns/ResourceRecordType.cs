using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns
{
    public enum ResourceRecordType : ushort
    {

        /// <summary>
        /// An IPv4 host address [RFC1035]
        /// </summary>
        A = 1,

        /// <summary>
        /// An authoritative name server RFC1035]
        /// </summary>
        NS = 2,
        MD = 3,         //  a mail destination (Obsolete - use MX)       [RFC1035]
        MF = 4,         //  a mail forwarder (Obsolete - use MX)         [RFC1035]
        CNAME = 5,      //  the canonical name for an alias              [RFC1035]
        SOA = 6,        //  marks the start of a zone of authority       [RFC1035]
        MB = 7,         //  a mailbox domain name (EXPERIMENTAL)         [RFC1035]
        MG = 8,         //  a mail group member (EXPERIMENTAL)           [RFC1035]
        MR = 9,         //  a mail rename domain name (EXPERIMENTAL)     [RFC1035]
        NULL = 10,      //  a null RR (EXPERIMENTAL)                    [RFC1035]
        WKS = 11,       //  a well known service description            [RFC1035]

        /// <summary>
        /// A domain name pointer [RFC1035]
        /// </summary>
        PTR = 12,
        HINFO = 13,     //  host information                            [RFC1035]
        MINFO = 14,     //  mailbox or mail list information            [RFC1035]

        /// <summary>
        /// Mail exchange [RFC1035]
        /// </summary>
        MX = 15,
        TXT = 16,       //  text strings                                [RFC1035]
        RP = 17,        //  for Responsible Person                      [RFC1183]
        AFSDB = 18,     //  for AFS Data Base location                  [RFC1183]
        X25 = 19,       //  for X.25 PSDN address                       [RFC1183]
        ISDN = 20,      //  for ISDN address                            [RFC1183]
        RT = 21,        //  for Route Through                           [RFC1183]
        NSAP = 22,      //  for NSAP address, NSAP style A record       [RFC1706]
        NSAP_PTR = 23,  //  for domain name pointer, NSAP style         [RFC1348] 
        SIG = 24,       //  for security signature                      [RFC4034][RFC3755][RFC2535]
        KEY = 25,       //  for security key                            [RFC4034][RFC3755][RFC2535]
        PX = 26,        //  X.400 mail mapping information              [RFC2163]
        GPOS = 27,      //  Geographical Position                       [RFC1712]

        /// <summary>
        /// An IPv6 host address [RFC3596]
        /// </summary>
        AAAA = 28,
        LOC = 29,       //  Location Information                        [RFC1876]
        NXT = 30,       //  Next Domain - OBSOLETE                      [RFC3755][RFC2535]
        EID = 31,       //  Endpoint Identifier                         [Patton]
        NIMLOC = 32,    //  Nimrod Locator                              [Patton]
        SRV = 33,       //  Server Selection                            [RFC2782]
        ATMA = 34,      //  ATM Address                                 [ATMDOC]
        NAPTR = 35,     //  Naming Authority Pointer                    [RFC2915][RFC2168]
        KX = 36,        //  Key Exchanger                               [RFC2230]
        CERT = 37,      //  CERT                                        [RFC4398]
        A6 = 38,        //  A6 (Experimental)                           [RFC3226][RFC2874]
        DNAME = 39,     //  DNAME                                       [RFC2672]
        SINK = 40,      //  SINK                                        [Eastlake]
        OPT = 41,       //  OPT                                         [RFC2671]
        APL = 42,       //  APL                                         [RFC3123]
        DS = 43,        //  Delegation Signer                           [RFC4034][RFC3658]
        SSHFP = 44,     //  SSH Key Fingerprint                         [RFC4255]
        IPSECKEY = 45,  //  IPSECKEY                                    [RFC4025]
        RRSIG = 46,     //  RRSIG                                       [RFC4034][RFC3755]
        NSEC = 47,      //  NSEC                                        [RFC4034][RFC3755]
        DNSKEY = 48,    //  DNSKEY                                      [RFC4034][RFC3755]
        DHCID = 49,     //  DHCID                                       [RFC4701]
        NSEC3 = 50,     //  NSEC3                                       [RFC5155]
        NSEC3PARAM = 51,  //  NSEC3PARAM                                  [RFC5155]

        HIP = 55,       //  Host Identity Protocol                      [RFC5205]
        NINFO = 56,     //  NINFO                                       [Reid]
        RKEY = 57,      //  RKEY                                        [Reid]

        SPF = 99,       //                                              [RFC4408]
        UINFO = 100,    //                                             [IANA-Reserved]
        UID = 101,      //                                             [IANA-Reserved]
        GID = 102,      //                                             [IANA-Reserved]
        UNSPEC = 103,   //                                             [IANA-Reserved]

        TKEY = 249,     //  Transaction Key                            [RFC2930]
        TSIG = 250,     //  Transaction Signature                      [RFC2845]
        IXFR = 251,     //  incremental transfer                       [RFC1995]
        AXFR = 252,     //  transfer of an entire zone                 [RFC1035]
        MAILB = 253,    //  mailbox-related RRs (MB, MG or MR)         [RFC1035]
        MAILA = 254,    //  mail agent RRs (Obsolete - see MX)         [RFC1035]
        ANY = 255,      //  A request for all records                  [RFC1035]

        TA = 32768,     //    DNSSEC Trust Authorities               [Weiler]           2005-12-13
        DLV = 32769,    //    DNSSEC Lookaside Validation            [RFC4431]

        Reserved = 65535
    }
}
