using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ScriptTrigger.CLI.BusinessLogic.Infrastructure
{
    public static class Internationalization
    {
        public static CultureInfo DefaultCulture { get; } = CultureInfo.CreateSpecificCulture("en-US");
    }
}
