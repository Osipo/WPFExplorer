﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public interface IMessage
    {
        string Message { get; }

        string ToString();
    }
}
