﻿using System;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class ObsoletePropertiesType
    {
        public string Property1 { get; set; }

        [Obsolete]
        public string ObsoleteProperty { get; set; }
    }
}