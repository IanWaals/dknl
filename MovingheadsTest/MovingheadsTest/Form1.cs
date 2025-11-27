using Dmx512UsbRs485; // Make sure this matches your namespace for the driver
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HydraBeamControl
{
    public partial class Form1 : Form
    {
        private Dmx512UsbRs485Driver driver;

        public Form1()
        {
            InitializeComponent();
            driver = new Dmx512UsbRs485Driver(); // create DMX driver instance
        }

        int Tilt1 = 0;
        int Tilt2 = 0;
        int Tilt3 = 0;
        int Tilt4 = 0;
        int pan1 = 0;
        int pan2 = 0;
        int pan3 = 0;
        int pan4 = 0;
        int red = 0;
        int blue = 0;
        int green = 0;
        int white = 0;
        int speed = 128;
        int dim1 = 0;
        int dim2 = 0;
        int dim3 = 0;
        int dim4 = 0;
        int strobe = 0;

        private void btnHeadsTilt_Click(object sender, EventArgs e)
        {
            MoveIt();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Tilt1 = trackBar1.Value;
            MoveIt();
        }
        private void MoveIt()
        {
            // Open the COM port (replace COM3 with your actual port)
            driver.DmxToDefault("COM5");

            // MASTER dimmer full
            driver.DmxLoadBuffer(1, 255, 512);

            // Strobe off
            driver.DmxLoadBuffer(2, (byte)strobe, 512);

            // RGBW full
            driver.DmxLoadBuffer(3, (byte)red, 512); // Red
            driver.DmxLoadBuffer(4, (byte)green, 512); // Green
            driver.DmxLoadBuffer(5, (byte)blue, 512); // Blue
            driver.DmxLoadBuffer(6, (byte)white, 512); // White

            // Head 1 – point Tilt
            driver.DmxLoadBuffer(7, (byte)pan1, 512);  // Pan center
            driver.DmxLoadBuffer(8, (byte)Tilt1, 512);    // Tilt Tilt
            driver.DmxLoadBuffer(9, (byte)dim1, 512);  // Dimmer full

            // Head 2 – point Tilt
            driver.DmxLoadBuffer(10, (byte)pan2, 512);
            driver.DmxLoadBuffer(11, (byte)Tilt2, 512);
            driver.DmxLoadBuffer(12, (byte)dim2, 512);

            // Head 3 – point Tilt
            driver.DmxLoadBuffer(13, (byte)pan3, 512);
            driver.DmxLoadBuffer(14, (byte)Tilt3, 512);
            driver.DmxLoadBuffer(15, (byte)dim3, 512);

            // Head 4 – point Tilt
            driver.DmxLoadBuffer(16, (byte)pan4, 512);
            driver.DmxLoadBuffer(17, (byte)Tilt4, 512);
            driver.DmxLoadBuffer(18, (byte)dim4, 512);

            // Speed / Strobe (slow)
            driver.DmxLoadBuffer(19, (byte)speed, 512);

            // Send all 19 channels to the lights
            driver.DmxSendCommand(19);

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Tilt2 = trackBar2.Value;
            MoveIt();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Tilt3 = trackBar3.Value;
            MoveIt();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            Tilt4 = trackBar4.Value;
            MoveIt();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            Tilt4 = trackBar5.Value;
            Tilt3 = trackBar5.Value;
            Tilt2 = trackBar5.Value;
            Tilt1 = trackBar5.Value;
            MoveIt();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            pan1 = trackBar6.Value;
            MoveIt();
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            pan2 = trackBar7.Value;
            MoveIt();
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            pan3 = trackBar8.Value;
            MoveIt();
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            pan4 = trackBar9.Value;
            MoveIt();
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            pan1 = trackBar10.Value;
            pan2 = trackBar10.Value;
            pan3 = trackBar10.Value;
            pan4 = trackBar10.Value;
            MoveIt();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            speed = trackBar11.Value;
            MoveIt();
        }

        private void w(object sender, KeyPressEventArgs e)
        {
            pan1 += 5;
            pan2 += 5;
            pan3 += +5;
            pan4 += 5;
        }

        private void trackBar12_Scroll(object sender, EventArgs e)
        {
            red = trackBar12.Value;
            MoveIt();
        }

        private void trackBar13_Scroll(object sender, EventArgs e)
        {
            green = trackBar13.Value;
            MoveIt();
        }

        private void trackBar14_Scroll(object sender, EventArgs e)
        {
            blue = trackBar14.Value;
            MoveIt();
        }

        private void trackBar15_Scroll(object sender, EventArgs e)
        {
            white = trackBar15.Value;
            MoveIt();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Random r = new Random();

            // Pattern 1 – startup
            speed = 80;
            red = 255; green = 0; blue = 80;
            pan1 = 0; pan2 = 45; pan3 = 90; pan4 = 135;
            Tilt1 = Tilt2 = Tilt3 = Tilt4 = 120;
            MoveIt();
            await Task.Delay(1500);

            // Pattern 2 – reverse sweep
            for (int i = 0; i < 4; i++)
            {
                pan1 = (pan1 + 45) % 180;
                pan2 = (pan2 + 45) % 180;
                pan3 = (pan3 + 45) % 180;
                pan4 = (pan4 + 45) % 180;
                Tilt1 = Tilt2 = Tilt3 = Tilt4 = 90 + (i * 10);

                speed = 40 + i * 10;
                MoveIt();
                await Task.Delay(700);
            }

            // Pattern 3 – random flashes
            for (int i = 0; i < 8; i++)
            {
                red = r.Next(256);
                green = r.Next(256);
                blue = r.Next(256);

                pan1 = r.Next(180);
                pan2 = r.Next(180);
                pan3 = r.Next(180);
                pan4 = r.Next(180);

                Tilt1 = r.Next(180);
                Tilt2 = r.Next(180);
                Tilt3 = r.Next(180);
                Tilt4 = r.Next(180);

                speed = r.Next(20, 150);

                MoveIt();
                await Task.Delay(r.Next(300, 900));
            }

            // Pattern 4 – circle rotation
            for (int i = 0; i < 360; i += 20)
            {
                pan1 = (i + 0) % 180;
                pan2 = (i + 45) % 180;
                pan3 = (i + 90) % 180;
                pan4 = (i + 135) % 180;

                Tilt1 = 100;
                Tilt2 = 80;
                Tilt3 = 120;
                Tilt4 = 95;

                speed = 60;

                MoveIt();
                await Task.Delay(120);
            }

            // Pattern 5 – strobe madness
            for (int i = 0; i < 10; i++)
            {
                red = 255; green = 255; blue = 255;
                speed = 200;
                MoveIt();
                await Task.Delay(80);

                red = 0; green = 0; blue = 0;
                MoveIt();
                await Task.Delay(80);
            }

            // Pattern 6 – slow fade & drift
            for (int i = 0; i <= 255; i += 5)
            {
                red = i;
                green = 255 - i;
                blue = (i * 2) % 255;

                pan1 = (pan1 + 2) % 180;
                pan2 = (pan2 + 3) % 180;
                pan3 = (pan3 + 4) % 180;
                pan4 = (pan4 + 5) % 180;

                Tilt1 = (Tilt1 + 1) % 180;
                Tilt2 = (Tilt2 + 2) % 180;
                Tilt3 = (Tilt3 + 3) % 180;
                Tilt4 = (Tilt4 + 4) % 180;

                speed = 30;

                MoveIt();
                await Task.Delay(60);
            }

            // Pattern 7 – all heads cross switch
            for (int i = 0; i < 4; i++)
            {
                pan1 = 0; pan2 = 180; pan3 = 90; pan4 = 45;
                Tilt1 = 50; Tilt2 = 110; Tilt3 = 150; Tilt4 = 70;
                MoveIt();
                await Task.Delay(500);

                pan1 = 180; pan2 = 0; pan3 = 45; pan4 = 90;
                Tilt1 = 140; Tilt2 = 80; Tilt3 = 60; Tilt4 = 100;
                MoveIt();
                await Task.Delay(500);
            }

            // Pattern 8 – wild random burst finale
            for (int i = 0; i < 20; i++)
            {
                red = r.Next(256);
                green = r.Next(256);
                blue = r.Next(256);

                pan1 = r.Next(180);
                pan2 = r.Next(180);
                pan3 = r.Next(180);
                pan4 = r.Next(180);

                Tilt1 = r.Next(180);
                Tilt2 = r.Next(180);
                Tilt3 = r.Next(180);
                Tilt4 = r.Next(180);

                speed = r.Next(50, 150);

                MoveIt();
                await Task.Delay(150);
            }

            // End with all LEDs white & centered
            speed = 80;
            red = green = blue = 255;
            pan1 = pan2 = pan3 = pan4 = 90;
            Tilt1 = Tilt2 = Tilt3 = Tilt4 = 100;
            MoveIt();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GrbTilt_Enter(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GrbTilt.Visible == true)
            {
                GrbTilt.Visible = false;
            }
            else
            {
                GrbTilt.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (GrbPan.Visible == true)
            {
                GrbPan.Visible = false;
            }
            else
            {
                GrbPan.Visible = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dim1 = 255;
            }
            else
            {
                dim1 = 0;
            }
            MoveIt();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                dim2 = 255;
            }
            else
            {
                dim2 = 0;
            }
            MoveIt();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                dim3 = 255;
            }
            else
            {
                dim3 = 0;
            }
            MoveIt();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                dim4 = 255;
            }
            else
            {
                dim4 = 0;
            }
            MoveIt();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (strobe == 0)
            {
                strobe = 255;
            }
            else
            {
                strobe = 0;
            }
            MoveIt();
        }

        private void trackBar16_Scroll(object sender, EventArgs e)
        {
            strobe = trackBar16.Value;
            MoveIt();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Tilt1 = Tilt2 = Tilt3 = Tilt4 = pan1 = pan2 = pan3 = pan4 = red = blue = green = white = dim1 = dim2 = dim3 = dim4 = strobe = 0;
            MoveIt();
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
        }

        private async void onoff_Click(object sender, EventArgs e)
        {
            if (dim1 + dim2 + dim3 + dim4 > 0)
            {
                dim1 = dim2 = dim3 = dim4 = 0;
                checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = false;
                MoveIt();
            }
            else
            {
                dim1 = 255;
                MoveIt();
                await Task.Delay(10);
                dim2 = 255;
                MoveIt();
                await Task.Delay(10);
                dim3 = 255;
                MoveIt();
                await Task.Delay(10);
                dim4 = 255;
                MoveIt();

                checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = true;
                MoveIt();
            }
        }

        private void btnHeadsUp_Click(object sender, EventArgs e)
        {

        }
    }
}
