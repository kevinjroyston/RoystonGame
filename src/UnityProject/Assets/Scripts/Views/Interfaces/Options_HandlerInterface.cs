﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Views.Interfaces
{
    public interface Options_HandlerInterface<T> : HandlerInterface<Dictionary<T, object>>
    {
        void UpdateValue(Dictionary<T, object> options);
    }
}
