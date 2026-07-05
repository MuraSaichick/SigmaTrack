using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Enums
{
    public enum IssueLinkType
    {
        RelatesTo = 0,   
        Blocks = 1,      
        IsBlockedBy = 2, 
        DependsOn = 3,    
        RequiredFor = 4,  
        Duplicates = 5,   
        IsDuplicatedBy = 6 
    }
}
