﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.DataAccess.Dbinitializer
{
    public interface IDbinitializer
    {
        Task InitializeAsync();
    }
}
