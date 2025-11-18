using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace DMXControl
{
    public partial class Form1 : Form
    {
        SerialPort dmxPort;
        byte[] dmxData = new byte[512]; // DMX has 512 channels
        Thread dmxThread;
        bool isRunning = false;

        // Store the actual values when lights are on
        private byte[] savedValues = new byte[9]; // Store values for channels 0-8
        private bool light1Enabled = true;
        private bool light2Enabled = true;
        private bool light3Enabled = true;

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

                // Start the continuous DMX transmission thread
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
                    Thread.Sleep(25); // ~40Hz refresh rate (typical for DMX)
                }
                catch (Exception ex)
                {
                    // Log error but continue trying
                    Console.WriteLine("DMX transmission error: " + ex.Message);
                }
            }
        }

        private void SendDMXFrame()
        {
            if (!dmxPort.IsOpen) return;

            try
            {
                // Generate BREAK by temporarily switching to slower baud rate
                dmxPort.BaudRate = 80000;
                dmxPort.Write(new byte[] { 0 }, 0, 1);

                // Return to normal DMX baud rate
                dmxPort.BaudRate = 250000;

                // Send START code (0x00)
                dmxPort.Write(new byte[] { 0 }, 0, 1);

                // Send all 512 DMX channel values
                dmxPort.Write(dmxData, 0, dmxData.Length);

                // Wait for transmission to complete
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
            // Stop the transmission thread
            isRunning = false;

            if (dmxThread != null && dmxThread.IsAlive)
            {
                dmxThread.Join(1000); // Wait up to 1 second for thread to finish
            }

            // Turn off all channels before closing
            Array.Clear(dmxData, 0, dmxData.Length);
            if (dmxPort.IsOpen)
            {
                SendDMXFrame();
                Thread.Sleep(50);
                dmxPort.Close();
            }
        }

        // Light 1 Controls (Channels 1-3)
        private void trbChannel1Iwaa_Scroll(object sender, EventArgs e)
        {
            savedValues[0] = (byte)trbChannel1iwaa.Value;
            if (light1Enabled)
            {
                dmxData[0] = savedValues[0];
            }

            if (lblChannel1Value != null)
            {
                lblChannel1Value.Text = $"Channel 1: {savedValues[0]}";
            }
        }

        private void btnToggleLight1_Click(object sender, EventArgs e)
        {
            light1Enabled = !light1Enabled;

            if (light1Enabled)
            {
                // Turn on - restore saved values
                dmxData[0] = savedValues[0];
                dmxData[1] = savedValues[1];
                dmxData[2] = savedValues[2];
                btnToggleLight1.Text = "Light 1: ON";
                btnToggleLight1.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                // Turn off - set to 0
                dmxData[0] = 0;
                dmxData[1] = 0;
                dmxData[2] = 0;
                btnToggleLight1.Text = "Light 1: OFF";
                btnToggleLight1.BackColor = System.Drawing.Color.LightGray;
            }
        }

        // Light 2 Controls (Channels 4-6)
        private void trbChannel2_Scroll(object sender, EventArgs e)
        {
            savedValues[3] = (byte)trbChannel2iwaa.Value;
            if (light2Enabled)
            {
                dmxData[3] = savedValues[3];
            }

            if (lblChannel2Value != null)
            {
                lblChannel2Value.Text = $"Channel 4: {savedValues[3]}";
            }
        }

        private void btnToggleLight2_Click(object sender, EventArgs e)
        {
            light2Enabled = !light2Enabled;

            if (light2Enabled)
            {
                // Turn on - restore saved values
                dmxData[3] = savedValues[3];
                dmxData[4] = savedValues[4];
                dmxData[5] = savedValues[5];
                btnToggleLight2.Text = "Light 2: ON";
                btnToggleLight2.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                // Turn off - set to 0
                dmxData[3] = 0;
                dmxData[4] = 0;
                dmxData[5] = 0;
                btnToggleLight2.Text = "Light 2: OFF";
                btnToggleLight2.BackColor = System.Drawing.Color.LightGray;
            }
        }

        // Light 3 Controls (Channels 7-9)
        private void trbChannel3iwaa_Scroll(object sender, EventArgs e)
        {
            savedValues[6] = (byte)trbChannel3iwaa.Value;
            if (light3Enabled)
            {
                dmxData[6] = savedValues[6];
            }

            if (lblChannel3Value != null)
            {
                lblChannel3Value.Text = $"Channel 7: {savedValues[6]}";
            }
        }

        private void btnToggleLight3_Click(object sender, EventArgs e)
        {
            light3Enabled = !light3Enabled;

            if (light3Enabled)
            {
                // Turn on - restore saved values
                dmxData[6] = savedValues[6];
                dmxData[7] = savedValues[7];
                dmxData[8] = savedValues[8];
                btnToggleLight3.Text = "Light 3: ON";
                btnToggleLight3.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                // Turn off - set to 0
                dmxData[6] = 0;
                dmxData[7] = 0;
                dmxData[8] = 0;
                btnToggleLight3.Text = "Light 3: OFF";
                btnToggleLight3.BackColor = System.Drawing.Color.LightGray;
            }
        }
    }
}