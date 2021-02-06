using System;
using System.Collections.Generic;
using System.Text;
using TicketProvider.Net.Mail;

namespace TicketProvider.Net.Mail.Sinks
{

    /// <summary>
    /// The IMessageSink provides a common interface
    /// </summary>
    public interface IMessageSink : IDisposable
    {

        /// <summary>
        /// Send the passed message to the target of this message sink.
        /// </summary>
        /// <param name="message"></param>
        void Send(MailMessage message);

        /// <summary>
        /// Opens the connection to the underlying target of the message sink.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection to the underlying target of the message sink.
        /// </summary>
        void Close();

        /// <summary>
        /// Releases any unmanaged resources hold by the message sink.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Gets if the message sink is open.
        /// </summary>
        bool IsOpen { get; }
    }
}
