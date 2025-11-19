using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clubbing_for_coders
{
    public partial class Form1 : Form
    {
        SerialPort dmxPort;
        byte[] dmxData = new byte[512]; // DMX has 512 channels
        Thread dmxThread;
        bool isRunning = false;

        private byte[] savedDmxData = new byte[9];
        bool lightsTogglePower = true;
        bool toggleFlashing = false;

        public Form1()
        {
            InitializeComponent();
            dmxPort = new SerialPort("COM13", 250000, Parity.None, 8, StopBits.Two);
        }

        private void btnOpenControllerIwaa_Click(object sender, EventArgs e)
        {
            tbcPagesIwaa.SelectedIndex = 1;
        }

        private void btnOpenShowsIwaa_Click(object sender, EventArgs e)
        {
            tbcPagesIwaa.SelectedIndex = 2;
        }

        private void btnOpenTimelineIwaa_Click(object sender, EventArgs e)
        {
            tbcPagesIwaa.SelectedIndex = 3;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbcPagesIwaa.Appearance = TabAppearance.FlatButtons;
            tbcPagesIwaa.ItemSize = new Size(0, 1);
            tbcPagesIwaa.SizeMode = TabSizeMode.Fixed;
            trbFlashIntervalIwaa.Enabled = false;

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

        private void btnTogglePowerAllParIwaa_Click(object sender, EventArgs e)
        {
            if (lightsTogglePower)
            {
                // Turn off flashing first if it's active
                if (toggleFlashing)
                {
                    toggleFlashing = false;
                    btnFlashLightsIwaa.Text = "Make the lights flash";
                    lblFlashingIwaa.Text = "Not flashing";
                    trbFlashIntervalIwaa.Enabled = false;
                }

                dmxData[0] = 0;
                dmxData[1] = 0;
                dmxData[2] = 0;

                trbRedParIwaa.Enabled = false;
                trbGreenParIwaa.Enabled = false;
                trbBlueParIwaa.Enabled = false;

                btnTogglePowerAllParIwaa.Text = "Turn On";
                lightsTogglePower = false;
            }
            else
            {
                dmxData[0] = (byte)savedDmxData[0];
                dmxData[1] = (byte)savedDmxData[1];
                dmxData[2] = (byte)savedDmxData[2];

                trbRedParIwaa.Enabled = true;
                trbGreenParIwaa.Enabled = true;
                trbBlueParIwaa.Enabled = true;

                btnTogglePowerAllParIwaa.Text = "Turn Off";
                lightsTogglePower = true;

                SendDMXFrame();
            }
        }

        private void trbRedParIwaa_Scroll(object sender, EventArgs e)
        {
            // Update DMX channel 1 (array index 0)
            dmxData[0] = (byte)trbRedParIwaa.Value;
            savedDmxData[0] = (byte)dmxData[0];

            // Update the label to show current value
            if (lblRedValueIwaa != null)
            {
                lblRedValueIwaa.Text = $"Red: {dmxData[0]}";
            }

            // No need to call SendDMX() here - the background thread handles it
        }

        private void trbGreenParIwaa_Scroll(object sender, EventArgs e)
        {
            // Update DMX channel 1 (array index 0)
            dmxData[1] = (byte)trbGreenParIwaa.Value;
            savedDmxData[1] = (byte)dmxData[1];

            // Update the label to show current value
            if (lblGreenValue != null)
            {
                lblGreenValue.Text = $"Green: {dmxData[1]}";
            }

            // No need to call SendDMX() here - the background thread handles it
        }

        private void trbBlueParIwaa_Scroll(object sender, EventArgs e)
        {
            // Update DMX channel 1 (array index 0)
            dmxData[2] = (byte)trbBlueParIwaa.Value;
            savedDmxData[2] = (byte)dmxData[2];

            // Update the label to show current value
            if (lblBlueValue != null)
            {
                lblBlueValue.Text = $"Blue: {dmxData[2]}";
            }

            // No need to call SendDMX() here - the background thread handles it
        }

        private void btnFlashLightsIwaa_Click(object sender, EventArgs e)
        {
            if (toggleFlashing == false)
            {
                toggleFlashing = true;
                btnFlashLightsIwaa.Text = "Stop Flashing";
                lblFlashingIwaa.Text = "Flashing";
                trbFlashIntervalIwaa.Enabled = true;
                LightsFlashing();
            }
            else
            {
                toggleFlashing = false;
                btnFlashLightsIwaa.Text = "Make the lights flash";
                lblFlashingIwaa.Text = "Not flashing";
                trbFlashIntervalIwaa.Enabled = false;
            }
        }

        private async void LightsFlashing()
        {
            // Store the current RGB values from trackbars
            byte flashRed = (byte)trbRedParIwaa.Value;
            byte flashGreen = (byte)trbGreenParIwaa.Value;
            byte flashBlue = (byte)trbBlueParIwaa.Value;

            // Disable trackbars during flashing
            trbRedParIwaa.Enabled = false;
            trbGreenParIwaa.Enabled = false;
            trbBlueParIwaa.Enabled = false;

            // Change button text to indicate flashing is active
            btnFlashLightsIwaa.Text = "Flashing... (Click to Stop)";

            // Flash loop - runs until toggleFlashing is set to false
            while (toggleFlashing)
            {
                // Turn lights ON with current colors
                dmxData[0] = flashRed;
                dmxData[1] = flashGreen;
                dmxData[2] = flashBlue;

                await Task.Delay(trbFlashIntervalIwaa.Value); // Wait (lights on)

                if (!toggleFlashing) break; // Check if stopped

                // Turn lights OFF
                dmxData[0] = 0;
                dmxData[1] = 0;
                dmxData[2] = 0;

                await Task.Delay(trbFlashIntervalIwaa.Value); // Wait (lights off)
            }

            // Only restore original values if lights are supposed to be ON
            if (lightsTogglePower)
            {
                dmxData[0] = flashRed;
                dmxData[1] = flashGreen;
                dmxData[2] = flashBlue;
            }
            else
            {
                // Keep lights off
                dmxData[0] = 0;
                dmxData[1] = 0;
                dmxData[2] = 0;
            }

            // Re-enable trackbars
            trbRedParIwaa.Enabled = true;
            trbGreenParIwaa.Enabled = true;
            trbBlueParIwaa.Enabled = true;

            // Reset button
            btnFlashLightsIwaa.Text = "Make the lights flash";
            btnFlashLightsIwaa.Click += btnFlashLightsIwaa_Click;
        }

        private void trbFlashIntervalIwaa_Scroll(object sender, EventArgs e)
        {
            lblFlashIntervalIwaa.Text = trbFlashIntervalIwaa.Value.ToString();
        }
    }
}
