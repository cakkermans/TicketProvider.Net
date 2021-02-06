using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{
    public enum SmtpCommandType
    {

        HELO,
        EHLO,
        MAIL,
        RCPT,
        DATA,
        DATA_CONTENT,
        RSET,
        NOOP,
        QUIT,
        VRFY,
        EXPN,
        HELP,

        UNKNOWN,
        INVALID,
    }
}
