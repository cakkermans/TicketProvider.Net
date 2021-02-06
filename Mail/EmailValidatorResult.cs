using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The EmailValidationResult class represents the results of a deep email validation operation.
    /// </summary>
    public class EmailValidatorResult
    {

        #region Variables

        private EmailAddress _emailAddress;

        private EmailValidatorResultState _syntaxValid;
        private EmailValidatorResultState _mxDefined;
        private EmailValidatorResultState _mxReachable;
        private EmailValidatorResultState _deliverable;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the email address validated.
        /// </summary>
        public EmailAddress EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Gets / sets if the syntax of the validated email address is valid.
        /// </summary>
        public EmailValidatorResultState SynaxValid
        {
            get { return _syntaxValid; }
            set { _syntaxValid = value; }
        }

        /// <summary>
        /// Gets / sets if one or more mail exchanges are defined for the validated email address.
        /// </summary>
        public EmailValidatorResultState MxDefined
        {
            get { return _mxDefined; }
            set { _mxDefined = value; }
        }

        /// <summary>
        /// Gets / sets if any of the mail exchanges defined for the validated email address are reachable.
        /// </summary>
        public EmailValidatorResultState MxReachable
        {
            get { return _mxReachable; }
            set { _mxReachable = value; }
        }

        /// <summary>
        /// Gets / sets if validated email address is reported to be deliverable by the mail exchange for the validated
        /// email address.
        /// </summary>
        public EmailValidatorResultState Deliverable
        {
            get { return _deliverable; }
            set { _deliverable = value; }
        }

        /// <summary>
        /// Gets a global boolean value indicating if the validation results if the validated email address is valid and should be
        /// accepted, based on the individual validation results.
        /// </summary>
        public bool Valid
        {
            get
            {
                if (_syntaxValid == EmailValidatorResultState.Failed)
                    return false;
                if (_mxDefined == EmailValidatorResultState.Failed)
                    return false;
                if (_mxReachable == EmailValidatorResultState.Failed)
                    return false;
                if (_deliverable == EmailValidatorResultState.Failed)
                    return false;
                return true;
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instace of the EmailValidationResult class for the specified email address with 
        /// all results set to 'Unknown'.
        /// </summary>
        public EmailValidatorResult(EmailAddress emailAddress)
        {

            _emailAddress = emailAddress;

            _deliverable = EmailValidatorResultState.Unknown;
            _mxDefined = EmailValidatorResultState.Unknown;
            _mxReachable = EmailValidatorResultState.Unknown;
            _syntaxValid = EmailValidatorResultState.Unknown;
        }

        #endregion
    }
}
