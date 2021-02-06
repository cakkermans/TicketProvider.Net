using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{

    #region RFC 3462 Specifications

/*

2. Status Code Structure


   This document defines a new set of status codes to report mail system
   conditions.  These status codes are used for media and language
   independent status reporting.  They are not intended for system
   specific diagnostics.

   The syntax of the new status codes is defined as:

      status-code = class "." subject "." detail

      class = "2"/"4"/"5"

      subject = 1*3digit

      detail = 1*3digit

   White-space characters and comments are NOT allowed within a status-
   code.  Each numeric sub-code within the status-code MUST be expressed
   without leading zero digits.

   Status codes consist of three numerical fields separated by ".".  The
   first sub-code indicates whether the delivery attempt was successful.
   The second sub-code indicates the probable source of any delivery
   anomalies, and the third sub-code indicates a precise error
   condition.

   Example:  2.1.23

   The code space defined is intended to be extensible only by standards
   track documents.  Mail system specific status codes should be mapped
   as close as possible to the standard status codes.  Servers should
   send only defined, registered status codes.  System specific errors
   and diagnostics should be carried by means other than status codes.

   New subject and detail codes will be added over time.  Because the
   number space is large, it is not intended that published status codes
   will ever be redefined or eliminated.  Clients should preserve the
   extensibility of the code space by reporting the general error
   described in the subject sub-code when the specific detail is
   unrecognized.
*/

    #endregion

    /// <summary>
    /// http://tools.ietf.org/html/rfc3463
    /// </summary>
    public struct SmtpExtendedReplyCode
    {

        
    }

    /// <summary>
    /// The class sub-code provides a broad classification of the status.
    /// </summary>
    public enum SmtpExtendedReplyCodeClass : int
    {

        /// <summary>
        /// 2.XXX.XXX   Success
        /// </summary>
        /// <remarks>
        /// Success specifies that the DSN is reporting a positive delivery
        /// action.  Detail sub-codes may provide notification of
        /// transformations required for delivery.
        /// </remarks>
        Success = 2,
        
        /// <summary>
        /// 4.XXX.XXX   Persistent Transient Failure
        /// </summary>
        /// <remarks>
        /// A persistent transient failure is one in which the message as
        /// sent is valid, but persistence of some temporary condition has
        /// caused abandonment or delay of attempts to send the message.
        /// If this code accompanies a delivery failure report, sending in
        /// the future may be successful.
        /// </remarks>
        TransientFailure = 4,
        
        /// <summary>
        /// 5.XXX.XXX   Permanent Failure
        /// </summary>
        /// <remarks>
        /// A permanent failure is one which is not likely to be resolved
        /// by resending the message in the current form.  Some change to
        /// the message or the destination must be made for successful
        /// delivery.
        /// </remarks>
        PermanentFailure = 5
    }

    /// <summary>
    /// The subject sub-code classifies the status.  This value applies to
    /// each of the three classifications.  The subject sub-code, if
    /// recognized, must be reported even if the additional detail provided
    /// by the detail sub-code is not recognized.
    /// </summary>
    public enum SmtpExtendedReplyCodeSubject : int
    {

        /// <summary>
        /// X.0.XXX   Other or Undefined Status
        /// </summary>
        /// <remarks>
        /// There is no additional subject information available.
        /// </remarks>
        OtherOrUndefined = 0,

        /// <summary>
        /// X.1.XXX Addressing Status
        /// </summary>
        /// <remarks>
        /// The address status reports on the originator or destination
        /// address.  It may include address syntax or validity.  These
        /// errors can generally be corrected by the sender and retried.
        /// </remarks>
        Addressing = 1,

        /// <summary>
        /// X.2.XXX Mailbox Status
        /// </summary>
        /// <remarks>
        /// Mailbox status indicates that something having to do with the
        /// mailbox has caused this DSN.  Mailbox issues are assumed to be
        /// under the general control of the recipient.
        /// </remarks>
        Mailbox = 2,

        /// <summary>
        /// X.3.XXX Mail System Status
        /// </summary>
        /// <remarks>
        /// Mail system status indicates that something having to do with
        /// the destination system has caused this DSN.  System issues are
        /// assumed to be under the general control of the destination
        /// system administrator.
        /// </remarks>
        MailSystem = 3,

        /// <summary>
        /// X.4.XXX Network and Routing Status
        /// </summary>
        /// <remarks>
        /// The networking or routing codes report status about the
        /// delivery system itself.  These system components include any
        /// necessary infrastructure such as directory and routing
        /// services.  Network issues are assumed to be under the control
        /// of the destination or intermediate system administrator.
        /// </summary>
        NetworkAndRouting = 4,

        /// <summary>
        /// X.5.XXX Mail Delivery Protocol Status
        /// </summary>
        /// <remarks>
        /// The mail delivery protocol status codes report failures
        /// involving the message delivery protocol.  These failures
        /// include the full range of problems resulting from
        /// implementation errors or an unreliable connection.
        /// </remarks>
        MailDeliveryProtocol = 5,
        
        /// <summary>
        /// X.6.XXX Message Content or Media Status
        /// </summary>
        /// <remarks>
        /// The message content or media status codes report failures
        /// involving the content of the message.  These codes report
        /// failures due to translation, transcoding, or otherwise
        /// unsupported message media.  Message content or media issues are
        /// under the control of both the sender and the receiver, both of
        /// which must support a common set of supported content-types.
        /// </remarks>
        MessageContentOrMedia = 6,

        /// <summary>
        /// X.7.XXX Security or Policy Status
        /// </summary>
        /// <remarks>
        /// The security or policy status codes report failures involving
        /// policies such as per-recipient or per-host filtering and
        /// cryptographic operations.  Security and policy status issues
        /// are assumed to be under the control of either or both the
        /// sender and recipient.  Both the sender and recipient must
        /// permit the exchange of messages and arrange the exchange of
        /// necessary keys and certificates for cryptographic operations.
        /// </remarks>
        SecurityOfPolicy = 7
    }
}
