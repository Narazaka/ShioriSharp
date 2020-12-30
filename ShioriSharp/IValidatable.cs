﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShioriSharp {
    public interface IValidatable<T> {
        public bool Valid { get; }
        public T Validate();
    }
}
