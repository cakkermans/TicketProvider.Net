using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The EmailValidatorMxServer class describes the state of a MX server validated by and instance of the EmailValidator class.
    /// </summary>
    public class EmailValidatorMxServer
    {

        #region Variables

        private string _domainName;
        private IPAddress[] _ipAddresses;
        private EmailValidatorMxServerState _state;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the domain name of the MX server.
        /// </summary>
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }

        /// <summary>
        /// Gets / sets the IP-addresses of the MX server.
        /// </summary>
        public IPAddress[] IpAddresses
        {
            get { return _ipAddresses; }
            set { _ipAddresses = value; }
        }

        /// <summary>
        /// Gets / sets the validation state of the MX server.
        /// </summary>
        public EmailValidatorMxServerState State
        {
            get { return _state; }
            set { _state = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the EmailValidatorMxServer class.
        /// </summary>
        public EmailValidatorMxServer()
        {
            _domainName = string.Empty;
            _ipAddresses = new IPAddress[0];
            _state = EmailValidatorMxServerState.Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the EmailValidatorMxServer class with the specified domain name.
        /// </summary>
        /// <param name="domaiName"></param>
        public EmailValidatorMxServer(string domaiName)
            : this()
        {

            _domainName = domaiName;
        }

        /// <summary>
        /// Initializes a new instance of the EmailValidatorMxServer class with the specified domain name and ip addresses.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="ipAddresses"></param>
        public EmailValidatorMxServer(string domainName, IPAddress[] ipAddresses)
            : this(domainName)
        {

            _ipAddresses = ipAddresses;
        }

        #endregion
    }

    /// <summary>
    /// The EmailValidatorMxServerState enum indicates the status of a MX server used for email validation.
    /// </summary>
    public enum EmailValidatorMxServerState
    {

        Unknown,
        Failed,
        Rejected,
        Accepted,
    }
}
