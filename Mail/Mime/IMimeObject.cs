using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail.Mime
{
    public interface IMimeObject
    {

        void Serialize(ImfWriter writer);
    }
}
