using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Threading;
using System.Net.Sockets;

namespace BlueToothConnectWindowsTest
{
    public partial class Form1 : Form
    {

        private BluetoothClient m_Blueclient = new BluetoothClient();
        private Dictionary<string, BluetoothAddress> m_deviceAddresses = new Dictionary<string, BluetoothAddress>();

        private BluetoothListener m_bluetoothListener;
        private Thread m_listenThread;
        private bool m_isConnected;
        private NetworkStream peerStream;

        public Form1()
        {
            InitializeComponent();

            m_listenThread = new Thread(ReceiveData);
            m_listenThread.Start();
        }

        private void ReceiveData()
        {
            try
            {
                Guid mGUID = Guid.Parse("00001101-0000-1000-8000-00805F9B34FB");
                m_bluetoothListener = new BluetoothListener(mGUID);
                m_bluetoothListener.Start();
                m_Blueclient = m_bluetoothListener.AcceptBluetoothClient();
                m_isConnected = true;
            }
            catch (Exception)
            {
                m_isConnected = false;
            }
            while (m_isConnected)
            {
                string receive = string.Empty;
                if (m_Blueclient == null)
                {
                    continue;
                }
                try
                {
                    peerStream = m_Blueclient.GetStream();
                    byte[] buffer = new byte[6];
                    peerStream.Read(buffer, 0, 6);
                    receive = Encoding.UTF8.GetString(buffer).ToString();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Thread.Sleep(100);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {

            this.lblMessage.Text = "";
            this.lblMessage.Visible = true;
            BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
            BuleRadio.Mode = RadioMode.Connectable;
            BluetoothDeviceInfo[] Devices = m_Blueclient.DiscoverDevices();
            lsbDevices.Items.Clear();
            m_deviceAddresses.Clear();
            foreach (BluetoothDeviceInfo device in Devices)
            {
                lsbDevices.Items.Add(device.DeviceName);

                m_deviceAddresses[device.DeviceName] = device.DeviceAddress;
            }

            this.lblMessage.Text = "搜索设备完成,搜索到" + lsbDevices.Items.Count + "个蓝牙设备";

        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                BluetoothAddress DeviceAddress = m_deviceAddresses[lsbDevices.SelectedItem.ToString()];
                m_Blueclient.SetPin(DeviceAddress, txtPwd.Text.Trim());
                m_Blueclient.Connect(DeviceAddress, BluetoothService.Handsfree);
                MessageBox.Show("配对成功。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

