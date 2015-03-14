﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using lasercom;

namespace lasercom.gpib
{
    class PrologixGPIBProvider : GPIBProvider
    {
        #region Constants
        const string USBTerminator = "\r\n";
        const char ControllerEscape = (char)27; // ESC character
        const string ControllerCommandInitiator = "++";
        const string AddressCommand = "addr";
        const string AssertControllerInChargeCommand = "ifc";
        const string ModeCommand = "mode";
        const string EOICommand = "eoi";
        const string EOSCommand = "eos";

        const int DeviceMode = 0;
        const int ControllerMode = 1;
        const int DisableEOI = 0;
        const int EnableEOI = 1;
        const int EOSCRLF = 0;
        const int EOSCR = 1;
        const int EOSLF = 2;
        const int EOSNone = 3;
        #endregion

        SerialPort _port;

        public string PortName
        {
            get
            {
                return _port.PortName;
            }
        }

        int Timeout { get; set; }
        const int DefaultTimeout = 500;

        public PrologixGPIBProvider(string PortName)
            : this(PortName, DefaultTimeout)
        {

        }

        public PrologixGPIBProvider(string PortName, int Timeout)
        {
            #region Serial port configuration
            
            _port = new SerialPort(PortName);
            _port.BaudRate = 115200;
            _port.DataBits = 8;
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.One;

            // RTS/CTS handshakings
            _port.Handshake = Handshake.RequestToSend;
            _port.DtrEnable = true;

            // Error handling
            _port.DiscardNull = false;
            _port.ParityReplace = 0;

            #endregion
            this.Timeout = Timeout;
            ControllerCommand(AssertControllerInChargeCommand); // Assert Controller-in-Charge.
            ControllerCommand(ModeCommand, ControllerMode); // Disable listen-only mode.
            ControllerCommand(EOICommand, EnableEOI); // Assert EOI after transmit GPIB data.
            ControllerCommand(EOSCommand, EOSCRLF); // Use CRLF as GPIB terminator.
        }

        void Open()
        {
            _port.Open();
        }

        void Close()
        {
            if (_port.IsOpen) _port.Close();
            Thread.Sleep(Constants.SerialPortCloseDelay); // Prevents subsequent call from interfering with close.
        }

        override protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
        }

        void ControllerCommand(string command)
        {
            string data = ControllerCommandInitiator + command + USBTerminator;
            Log.Debug("USB GPIB Controller Command: " + data);
            try
            {
                if (!_port.IsOpen) _port.Open();
                _port.Write(data);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        void ControllerCommand(string command, params int[] args)
        {
            string arglist = String.Join(" ", args);
            string data = ControllerCommandInitiator + command + " " + arglist + USBTerminator;
            Log.Debug("USB GPIB Controller Command: " + data);
            try
            {
                if (!_port.IsOpen) _port.Open();
                _port.Write(data);
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        override public void LoggedWrite(byte address, string command)
        {
            Log.Info("GPIB Command: " + command);
            try
            {
                if (!_port.IsOpen) _port.Open();
                ControllerCommand(AddressCommand, address);
                _port.Write(EscapeAndTerminate(command));
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }

        override public string LoggedQuery(byte address, string command)
        {
            Log.Info("GPIB Command: " + command);
            string buffer = null;
            try
            {
                if (!_port.IsOpen) _port.Open();
                ControllerCommand(AddressCommand, address);
                _port.Write(EscapeAndTerminate(command));
                buffer = ReadWithTimeout();
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
            return buffer;
        }

        string ReadWithTimeout()
        {
            return ReadWithTimeout(Timeout);
        }

        string ReadWithTimeout(int Timeout)
        {
            if (!_port.IsOpen) throw new InvalidOperationException("The specified port is not open");

            StringBuilder builder = new StringBuilder();
            DateTime lastRead = DateTime.Now;
            TimeSpan elapsedTime = new TimeSpan();

            // 2 second timespan
            TimeSpan TimeSpan = new TimeSpan(0, 0, 0, 0, Timeout);

            // Read from port until TIMEOUT time has elapsed since
            // last successful read
            while (TimeSpan.CompareTo(elapsedTime) > 0)
            {
                string buffer = _port.ReadExisting();

                if (buffer.Length > 0)
                {
                    builder.Append(buffer);
                    lastRead = DateTime.Now;
                }
                elapsedTime = DateTime.Now - lastRead;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Escapes GPIB command string for use with Prologix controller.
        /// CR (13), LF (10), ESC (27), + (43) characters will be escaped.
        /// </summary>
        /// <param name="s">GPIB command</param>
        /// <returns>Escaped string</returns>
        static string EscapeString(string s)
        {
            StringBuilder builder = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == (char)10 || s[i] == (char)13 || s[i] == (char)27 || s[i] == (char)43)
                {
                    builder.Append(ControllerEscape);   
                }
                builder.Append(s[i]);
            }
            return builder.ToString();
        }

        static string EscapeAndTerminate(string s)
        {
            return EscapeString(s) + USBTerminator;
        }
    }
}
