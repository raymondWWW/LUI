﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lasercom.ddg
{
    public interface IDigitalDelayGenerator
    {

        void SetADelay(double delay);
        void SetBDelay(double delay);
        void SetCDelay(double delay);
        void SetDDelay(double delay);

        double GetADelay();
        double GetBDelay();
        double GetCDelay();
        double GetDDelay();
    }
}
