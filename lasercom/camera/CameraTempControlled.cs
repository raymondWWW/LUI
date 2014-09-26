﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//#if x64
using ATMCD64CS;
//#else
//using ATMCD32CS;
//#endif

//  <summary>
//      Temperature controlled camera.
//  </summary>

namespace LUI
{

    public class CameraTempControlled:AndorCamera
    {

        public CameraTempControlled(String dir):base(dir)
        {
            EquilibrateTemperature(Constants.DefaultTemperature);
        }

        public void EquilibrateTemperature(int targetTemperature)
        {
            AndorSdk.CoolerON();
            AndorSdk.SetTemperature(targetTemperature);
            float currentTemperature = 0;
            AndorSdk.GetTemperatureF(ref currentTemperature);
            while ( Math.Abs(currentTemperature - targetTemperature) > Constants.TemperatureEps )
            {
                AndorSdk.GetTemperatureF(ref currentTemperature);
            }
        }

        public override void Close()
        {
            AndorSdk.CoolerOFF();
            AndorSdk.ShutDown();
        }

        public int[] GetCountsFvb()
        {
            return Acquire();
        }

        public int[] Acquire()
        {
            uint npx;
            if (this.ReadMode == Constants.ReadModeFVB)
            {
                npx = this.Width;
            }
            else if (this.ReadMode == Constants.ReadModeImage)
            {
                npx = this.Width * this.Height;
            }
            else
            {
                throw new NotImplementedException();
            }

            int[] data = new int[npx];
            AndorSdk.StartAcquisition();
            AndorSdk.WaitForAcquisition();
            uint ret = AndorSdk.GetAcquiredData(data, npx);
            return data;
        }
 
    }

}