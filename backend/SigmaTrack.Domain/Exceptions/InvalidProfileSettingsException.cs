using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Exceptions
{
    public class InvalidProfileSettingsException : DomainException
    {
        public InvalidProfileSettingsException(string message) : base(message)
        {
        }
    }
}
