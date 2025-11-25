using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

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

        private bool isPlayingShow = false;
        private CancellationTokenSource playShowCancellationToken;

        string connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=FunDatabasename;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
            dmxPort = new SerialPort("COM14", 250000, Parity.None, 8, StopBits.Two);
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
            LoadComboBox();
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
                turnOff();
            }
            else
            {
                turnOn();
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
                startFlashing();
            }
            else
            {
                stopFlashing();
            }
        }

        private async void LightsFlashing()
        {
            disableButtons();
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

        //color button
        #region

        private void btnColorRedIwaa_Click(object sender, EventArgs e)
        {
            turnRed();
        }

        private void btnColorGreenIwaa_Click(object sender, EventArgs e)
        {
            turnGreen();
        }
        private void btnColorBlueIwaa_Click(object sender, EventArgs e)
        {
            turnBlue();
        }
        private void btnColorWhiteIwaa_Click(object sender, EventArgs e)
        {
            turnWhite();
        }
        private void btnColorPurpleIwaa_Click(object sender, EventArgs e)
        {
            turnPurple();
        }
        private void btnColorOrangeIwaa_Click(object sender, EventArgs e)
        {
            turnOrange();
        }
        private void btnColorYellowIwaa_Click(object sender, EventArgs e)
        {
            turnYellow();
        }

        private void disableButtons()
        {
            btnColorRedIwaa.Enabled = false;
            btnColorGreenIwaa.Enabled = false;
            btnColorBlueIwaa.Enabled = false;
            btnColorWhiteIwaa.Enabled = false;
            btnColorPurpleIwaa.Enabled = false;
            btnColorOrangeIwaa.Enabled = false;
            btnColorYellowIwaa.Enabled = false;
        }

        private void enableButtons()
        {
            btnColorRedIwaa.Enabled = true;
            btnColorGreenIwaa.Enabled = true;
            btnColorBlueIwaa.Enabled = true;
            btnColorWhiteIwaa.Enabled = true;
            btnColorPurpleIwaa.Enabled = true;
            btnColorOrangeIwaa.Enabled = true;
            btnColorYellowIwaa.Enabled = true;
        }

        #endregion

        //show functions
        #region

        private void turnRed()
        {
            dmxData[0] = 255;  // Red
            dmxData[1] = 0;    // Green
            dmxData[2] = 0;    // Blue

            savedDmxData[0] = 255;
            savedDmxData[1] = 0;
            savedDmxData[2] = 0;

            trbRedParIwaa.Value = 255;
            trbGreenParIwaa.Value = 0;
            trbBlueParIwaa.Value = 0;

            // Update labels
            lblRedValueIwaa.Text = "Red: 255";
            lblGreenValue.Text = "Green: 0";
            lblBlueValue.Text = "Blue: 0";
        }

        private void turnGreen()
        {
            dmxData[0] = 0;  // Red
            dmxData[1] = 255;    // Green
            dmxData[2] = 0;    // Blue

            savedDmxData[0] = 0;
            savedDmxData[1] = 255;
            savedDmxData[2] = 0;

            trbRedParIwaa.Value = 0;
            trbGreenParIwaa.Value = 255;
            trbBlueParIwaa.Value = 0;

            // Update labels
            lblRedValueIwaa.Text = "Red: 0";
            lblGreenValue.Text = "Green: 255";
            lblBlueValue.Text = "Blue: 0";
        }

        private void turnBlue()
        {
            dmxData[0] = 0;  // Red
            dmxData[1] = 0;    // Green
            dmxData[2] = 255;    // Blue

            savedDmxData[0] = 0;
            savedDmxData[1] = 0;
            savedDmxData[2] = 255;

            trbRedParIwaa.Value = 0;
            trbGreenParIwaa.Value = 0;
            trbBlueParIwaa.Value = 255;

            // Update labels
            lblRedValueIwaa.Text = "Red: 0";
            lblGreenValue.Text = "Green: 0";
            lblBlueValue.Text = "Blue: 255";
        }

        private void turnWhite()
        {
            dmxData[0] = 255;  // Red
            dmxData[1] = 255;    // Green
            dmxData[2] = 255;    // Blue

            savedDmxData[0] = 255;
            savedDmxData[1] = 255;
            savedDmxData[2] = 255;

            trbRedParIwaa.Value = 255;
            trbGreenParIwaa.Value = 255;
            trbBlueParIwaa.Value = 255;

            // Update labels
            lblRedValueIwaa.Text = "Red: 255";
            lblGreenValue.Text = "Green: 255";
            lblBlueValue.Text = "Blue: 255";
        }

        private void turnPurple()
        {
            dmxData[0] = 255;  // Red
            dmxData[1] = 0;    // Green
            dmxData[2] = 255;    // Blue

            savedDmxData[0] = 255;
            savedDmxData[1] = 0;
            savedDmxData[2] = 255;

            trbRedParIwaa.Value = 255;
            trbGreenParIwaa.Value = 0;
            trbBlueParIwaa.Value = 255;

            // Update labels
            lblRedValueIwaa.Text = "Red: 255";
            lblGreenValue.Text = "Green: 0";
            lblBlueValue.Text = "Blue: 255";
        }

        private void turnOrange()
        {
            dmxData[0] = 204;  // Red
            dmxData[1] = 85;    // Green
            dmxData[2] = 0;    // Blue

            savedDmxData[0] = 204;
            savedDmxData[1] = 85;
            savedDmxData[2] = 0;

            trbRedParIwaa.Value = 204;
            trbGreenParIwaa.Value = 85;
            trbBlueParIwaa.Value = 0;

            // Update labels
            lblRedValueIwaa.Text = "Red: 204";
            lblGreenValue.Text = "Green: 85";
            lblBlueValue.Text = "Blue: 0";
        }

        private void turnYellow()
        {
            dmxData[0] = 255;  // Red
            dmxData[1] = 255;    // Green
            dmxData[2] = 0;    // Blue

            savedDmxData[0] = 255;
            savedDmxData[1] = 255;
            savedDmxData[2] = 0;

            trbRedParIwaa.Value = 255;
            trbGreenParIwaa.Value = 255;
            trbBlueParIwaa.Value = 0;

            // Update labels
            lblRedValueIwaa.Text = "Red: 255";
            lblGreenValue.Text = "Green: 255";
            lblBlueValue.Text = "Blue: 0";
        }

        private void turnOff()
        {
            disableButtons();
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

        private void turnOn()
        {
            enableButtons();
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

        private void startFlashing()
        {
            toggleFlashing = true;
            btnFlashLightsIwaa.Text = "Stop Flashing";
            lblFlashingIwaa.Text = "Flashing";
            trbFlashIntervalIwaa.Enabled = true;
            LightsFlashing();
        }

        private void stopFlashing()
        {
            toggleFlashing = false;
            btnFlashLightsIwaa.Text = "Make the lights flash";
            lblFlashingIwaa.Text = "Not flashing";
            trbFlashIntervalIwaa.Enabled = false;
            enableButtons();
        }

        #endregion

        private void LoadComboBox()
        {
            string query = "SELECT Id, name FROM savedShows"; // columns you want

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "Name"; // column shown to user
                comboBox1.ValueMember = "Id";     // underlying value
            }
        }

        private async void btnPlayIwaa_Click(object sender, EventArgs e)
        {
            if (isPlayingShow)
            {
                // Stop the currently playing show
                StopShow();
                return;
            }

            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a show to play.");
                return;
            }

            int showId = (int)comboBox1.SelectedValue;
            await PlayShow(showId);
        }

        private async Task PlayShow(int showId)
        {
            string connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=FunDatabasename;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            string query = "SELECT functionSequence FROM savedShows WHERE Id = @Id";

            string functionSequence = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", showId);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        functionSequence = result.ToString();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(functionSequence))
            {
                MessageBox.Show("No function sequence found for this show.");
                return;
            }

            // Parse the function sequence
            string[] functions = functionSequence.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

            // Start playing
            isPlayingShow = true;
            playShowCancellationToken = new CancellationTokenSource();
            btnPlayIwaa.Text = "STOP";

            try
            {
                foreach (string functionName in functions)
                {
                    if (playShowCancellationToken.Token.IsCancellationRequested)
                        break;

                    // Execute the function
                    await ExecuteFunction(functionName.Trim());

                    // Wait between functions (adjust delay as needed)
                    if (!functionName.Trim().Equals("startFlashing", StringComparison.OrdinalIgnoreCase))
                    {
                        await Task.Delay(2000, playShowCancellationToken.Token); // 2 second delay between functions
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Show was stopped
            }
            finally
            {
                StopShow();
            }
        }

        private async Task ExecuteFunction(string functionName)
        {
            switch (functionName.ToLower())
            {
                case "turnred":
                    turnRed();
                    break;
                case "turngreen":
                    turnGreen();
                    break;
                case "turnblue":
                    turnBlue();
                    break;
                case "turnwhite":
                    turnWhite();
                    break;
                case "turnpurple":
                    turnPurple();
                    break;
                case "turnorange":
                    turnOrange();
                    break;
                case "turnyellow":
                    turnYellow();
                    break;
                case "turnon":
                    turnOn();
                    break;
                case "turnoff":
                    turnOff();
                    break;
                case "startflashing":
                    startFlashing();
                    // Wait for flashing to complete
                    await Task.Delay(5000, playShowCancellationToken.Token);
                    stopFlashing();
                    // Add extra delay to ensure cleanup is complete
                    await Task.Delay(500, playShowCancellationToken.Token);
                    break;
                default:
                    MessageBox.Show($"Unknown function: {functionName}");
                    break;
            }
        }

        private void StopShow()
        {
            isPlayingShow = false;
            playShowCancellationToken?.Cancel();
            btnPlayIwaa.Text = "PLAY";

            // Stop flashing if it's active
            if (toggleFlashing)
            {
                stopFlashing();
            }
        }
    }
}
