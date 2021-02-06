using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{
    public enum SmtpReplyCode : int
    {

        /// <summary>
        /// System status, or system help reply
        /// </summary>
        SystemStatus = 211,
        
        /// <summary>
        /// Help message (Information on how to use the receiver or the
        /// meaning of a particular non-standard command; this reply is useful
        /// only to the human user)
        /// </summary>
        HelpMessage = 214,

        /// <summary>
        /// Service ready
        /// </summary>
        ServiceReady = 220,

        /// <summary>
        /// Service closing transmission channel
        /// </summary>
        ServiceClosing = 221,

        /// <summary>
        /// Requested mail action okay, completed
        /// </summary>
        Completed = 250,

        /// <summary>
        /// User not local; will forward to <forward-path> (See Section 3.4)
        /// </summary>
        UserNotLocalWillFoward = 251,

        /// <summary>
        /// Cannot VRFY user, but will accept message and attempt delivery
        /// (See Section 3.5.3)
        /// </summary>
        CannotVerifyUser = 252,

        /// <summary>
        /// Start mail input; end with CRLF.CRLF
        /// </summary>
        StartMailInput = 354,

        /// <summary>
        /// Service not available, closing transmission channel
        /// (This may be a reply to any command if the service knows it must
        /// shut down)
        /// </summary>
        ServiceNotAvailable = 421,

        /// <summary>
        /// Requested mail action not taken: mailbox unavailable (e.g.,
        /// mailbox busy or temporarily blocked for policy reasons)
        /// </summary>
        MailboxTemporarilyUnavailable = 450,

        /// <summary>
        /// Requested action aborted: local error in processing
        /// </summary>
        LocalProcessingError = 451,

        /// <summary>
        /// Requested action not taken: insufficient system storage
        /// </summary>
        InsufficientSystemStorage = 452,

        /// <summary>
        /// Server unable to accommodate parameters
        /// </summary>
        UnableToAccomodateParameters = 455,

        /// <summary>
        /// Syntax error, command unrecognized (This may include errors such
        /// as command line too long)
        /// </summary>
        CommandSyntaxError = 500,

        /// <summary>
        /// Syntax error in parameters or arguments
        /// </summary>
        ParameterSyntaxError = 501,

        /// <summary>
        /// Command not implemented (see Section 4.2.4)
        /// </summary>
        CommandNotImplemented = 502,

        /// <summary>
        /// Bad sequence of commands
        /// </summary>
        BadSequence = 503,

        /// <summary>
        /// Command parameter not implemented
        /// </summary>
        ParameterNotImplemented = 504,

        /// <summary>
        /// Requested action not taken: mailbox unavailable (e.g., mailbox
        /// not found, no access, or command rejected for policy reasons)
        /// </summary>
        MailboxUnavailable = 550,

        /// <summary>
        /// User not local; please try forward-path (See Section 3.4)
        /// </summary>
        UserNotLocal = 551,

        /// <summary>
        /// Requested mail action aborted: exceeded storage allocation
        /// </summary>
        ExceededStorageAllocation = 552,

        /// <summary>
        /// Requested action not taken: mailbox name not allowed (e.g.,
        /// mailbox syntax incorrect)
        /// </summary>
        MailboxNameNotAllowed = 553,

        /// <summary>
        /// Transaction failed (Or, in the case of a connection-opening
        /// response, "No SMTP service here")
        /// </summary>
        TransactionFailed = 554,

        /// <summary>
        /// MAIL FROM/RCPT TO parameters not recognized or not implemented
        /// </summary>
        ParametersNotRecognized = 555,
    }
}
