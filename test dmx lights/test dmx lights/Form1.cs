using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace DMXControl
{
    public partial class Form1 : Form
    {
        SerialPort dmxPort;
        byte[] dmxData = new byte[512];
        Thread dmxThread;
        bool isRunning = false;

        // Light enable states
        bool light1Enabled = true;
        bool light2Enabled = true;
        bool light3Enabled = true;

        // DMX start addresses for each light
        int light1StartAddress = 1;  // Channels 1-3
        int light2StartAddress = 5;  // Channels 5-7
        int light3StartAddress = 3;  // Channels 3-5

        public Form1()
        {
            InitializeComponent();
            dmxPort = new SerialPort("COM12", 250000, Parity.None, 8, StopBits.Two);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                dmxPort.Open();
                isRunning = true;
                dmxThread = new Thread(DMXTransmissionLoop);
                dmxThread.IsBackground = true;
                dmxThread.Start();
                MessageBox.Show("DMX Port Opened and transmission started!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening DMX port: " + ex.Message);
            }
        }

        private void DMXTransmissionLoop()
        {
            while (isRunning && dmxPort.IsOpen)
            {
                try
                {
                    SendDMXFrame();
                    Thread.Sleep(25);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DMX transmission error: " + ex.Message);
                }
            }
        }

        private void SendDMXFrame()
        {
            if (!dmxPort.IsOpen) return;

            try
            {
                dmxPort.BaudRate = 80000;
                dmxPort.Write(new byte[] { 0 }, 0, 1);
                dmxPort.BaudRate = 250000;
                dmxPort.Write(new byte[] { 0 }, 0, 1);
                dmxPort.Write(dmxData, 0, dmxData.Length);
                while (dmxPort.BytesToWrite != 0) ;
                Thread.Sleep(20);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SendDMXFrame: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            if (dmxThread != null && dmxThread.IsAlive)
            {
                dmxThread.Join(1000);
            }
            Array.Clear(dmxData, 0, dmxData.Length);
            if (dmxPort.IsOpen)
            {
                SendDMXFrame();
                Thread.Sleep(50);
                dmxPort.Close();
            }
        }

        // Helper method to update light channels
        private void UpdateLightChannels(int startAddress, byte red, byte green, byte blue, bool enabled)
        {
            if (!enabled)
            {
                // Turn off light by setting all channels to 0
                dmxData[startAddress - 1] = 0;
                dmxData[startAddress] = 0;
                dmxData[startAddress + 1] = 0;
            }
            else
            {
                // Set RGB values
                dmxData[startAddress - 1] = red;
                dmxData[startAddress] = green;
                dmxData[startAddress + 1] = blue;
            }
        }

        // Channel 1 (Red) slider
        private void trbChannel1Iwaa_Scroll(object sender, EventArgs e)
        {
            byte redValue = (byte)trbChannel1iwaa.Value;
            byte greenValue = dmxData[light1StartAddress];
            byte blueValue = dmxData[light1StartAddress + 1];

            UpdateLightChannels(light1StartAddress, redValue, greenValue, blueValue, light1Enabled);

            if (lblChannel1Value != null)
            {
                lblChannel1Value.Text = $"Light 1 Red: {redValue}";
            }
        }

        // Channel 2 (Green) slider
        private void trbChannel2_Scroll(object sender, EventArgs e)
        {
            byte redValue = dmxData[light1StartAddress - 1];
            byte greenValue = (byte)trbChannel2iwaa.Value;
            byte blueValue = dmxData[light1StartAddress + 1];

            UpdateLightChannels(light1StartAddress, redValue, greenValue, blueValue, light1Enabled);

            if (lblChannel2Value != null)
            {
                lblChannel2Value.Text = $"Light 1 Green: {greenValue}";
            }
        }

        // Channel 3 (Blue) slider
        private void trbChannel3iwaa_Scroll(object sender, EventArgs e)
        {
            byte redValue = dmxData[light1StartAddress - 1];
            byte greenValue = dmxData[light1StartAddress];
            byte blueValue = (byte)trbChannel3iwaa.Value;

            UpdateLightChannels(light1StartAddress, redValue, greenValue, blueValue, light1Enabled);

            if (lblChannel3Value != null)
            {
                lblChannel3Value.Text = $"Light 1 Blue: {blueValue}";
            }
        }

        // Light 1 toggle button
        private void btnLight1ToggleIwaa_Click(object sender, EventArgs e)
        {
            light1Enabled = !light1Enabled;
            btnLight1ToggleIwaa.Text = light1Enabled ? "Light 1: ON" : "Light 1: OFF";
            btnLight1ToggleIwaa.BackColor = light1Enabled ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightGray;

            // Update light with current slider values
            UpdateLightChannels(light1StartAddress,
                dmxData[light1StartAddress - 1],
                dmxData[light1StartAddress],
                dmxData[light1StartAddress + 1],
                light1Enabled);
        }

        // Light 2 toggle button
        private void btnLight2ToggleIwaa_Click(object sender, EventArgs e)
        {
            light2Enabled = !light2Enabled;
            btnLight2ToggleIwaa.Text = light2Enabled ? "Light 2: ON" : "Light 2: OFF";
            btnLight2ToggleIwaa.BackColor = light2Enabled ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightGray;

            // Copy light 1 color to light 2
            UpdateLightChannels(light2StartAddress,
                dmxData[light1StartAddress - 1],
                dmxData[light1StartAddress],
                dmxData[light1StartAddress + 1],
                light2Enabled);
        }

        // Light 3 toggle button
        private void btnLight3ToggleIwaa_Click(object sender, EventArgs e)
        {
            light3Enabled = !light3Enabled;
            btnLight3ToggleIwaa.Text = light3Enabled ? "Light 3: ON" : "Light 3: OFF";
            btnLight3ToggleIwaa.BackColor = light3Enabled ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightGray;

            // Copy light 1 color to light 3
            UpdateLightChannels(light3StartAddress,
                dmxData[light1StartAddress - 1],
                dmxData[light1StartAddress],
                dmxData[light1StartAddress + 1],
                light3Enabled);
        }
    }
}