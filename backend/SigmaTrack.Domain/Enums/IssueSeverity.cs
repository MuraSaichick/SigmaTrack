using System;
using System.Collections.Generic;
using System.Text;

namespace SigmaTrack.Domain.Enums
{
    public enum IssueSeverity
    {
        Critical = 0,    // Система не работает
        Major = 1,       // Основной функционал сломан
        Minor = 2,       // Незначительная проблема
        Trivial = 3,     // Косметическая проблема
        Enhancement = 4  // Улучшение
    }
}
