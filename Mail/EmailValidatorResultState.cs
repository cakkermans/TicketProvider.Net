using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// Describes the validation state of an object validated by the EmailValidator.
    /// </summary>
    public enum EmailValidatorResultState
    {
        Unknown,

        /// <summary>
        /// The validation failed.
        /// </summary>
        Failed,
        Success,
    }
}
