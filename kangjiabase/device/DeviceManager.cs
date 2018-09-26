namespace kangjiabase
{
    using System;
    using System.IO.Ports;
    using System.Threading;
    using System.Collections.Generic;

    public class DeviceManager
    {
        public CommandManager CmdMngr;
        public Enum_DeviceStage CurrentStage = Enum_DeviceStage.Waiting;
        public SerialPort Port = new SerialPort();
        public int PortBaudRate = 0x1c200;
        public int PortDataBits = 8;
        public Handshake PortHandshake = Handshake.None;
        public string PortName = "COM1";
        public Parity PortParity = Parity.None;
        public StopBits PortStopBits = StopBits.One;

        public DeviceManager()
        {
            this.Port.DataReceived += new SerialDataReceivedEventHandler(this.DataReceviedHandler);
            this.CmdMngr = new CommandManager(this);
           
            this.CmdMngr.Clear();
        }

        private void DataReceviedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            
            SerialPort port = (SerialPort) sender;
            if (port.BytesToRead < 0)
            {
                return;
            }
            List<byte> alist = new List<byte>();
            while (true)
            {

                int listLeng = port.BytesToRead;
                if (listLeng <= 0)
                    break;
                
                byte[] newbye = new byte[listLeng];
                port.Read(newbye, 0, listLeng);
                alist.AddRange(newbye);
                port.DiscardInBuffer();
                if (alist.Count >= 300)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            byte[] buffer = alist.ToArray();
            //if (buffer.Length >= 32) {
            //    string x = "aa";
            //}
            ////byte[] buffer = new byte[port.BytesToRead];
            ////port.Read(buffer, 0, buffer.Length);
            ////this.CmdMngr.Push(buffer);
            this.CmdMngr.ProcessBuffer(buffer);
        }

        public void Init()
        {
            
            this.Port.PortName = this.PortName;
            this.Port.BaudRate = this.PortBaudRate;
            this.Port.Parity = this.PortParity;
            this.Port.StopBits = this.PortStopBits;
            this.Port.DataBits = this.PortDataBits;
            this.Port.Handshake = this.PortHandshake;
            this.Port.ReadBufferSize = 1024;
        }

        public bool Start()
        {
            bool flag = true;
            try
            {
                if (this.Port.IsOpen)
                {
                    this.Stop();
                    Thread.Sleep(500);
                }
                else
                {
                    this.Init();
                    this.Port.Open();
                }
                try
                {
                    this.Port.DiscardInBuffer();
                }
                catch { }
            }
            catch (Exception exception)
            {
                flag = false;
               // Log.WriteLog(exception.Message);
            }
            return flag;
        }

        public void Stop()
        {
            try
            {
                this.Port.Close();
            }
            catch
            {
            }
        }
        public void StopAll()
        {
            //try
            //{
            //    this.Port.DataReceived -= new SerialDataReceivedEventHandler(this.DataReceviedHandler);
            //}
            //catch (Exception exception)
            //{
            //    Functions.WriteLog(exception.Message);
            //}
        }
        public void Write(byte[] pData)
        {
            try
            {
                this.Port.Write(pData, 0, pData.Length);
            }
            catch (Exception exception)
            {
               // Log.WriteLog(exception.Message);
            }
        }

        public void Write(Command pCmd)
        {
            try
            {
                byte[] data = pCmd.GetData();
                this.Port.Write(data, 0, data.Length);
            }
            catch (Exception exception)
            {
               // Log.WriteLog(exception.Message);
            }
        }

        public enum Enum_DeviceStage
        {
            Waiting,
            Active
        }
    }
}

