using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GAuth
{
    public partial class Form1 : Form
    {

        private string keyString = null;
        private string totp = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            keyString = Properties.Settings.Default.Key;
            if (keyString == string.Empty) keyString = "MFRGGZDFGAYTEMZU"; // abcde01234
            toolStripTextBox1.Text = keyString;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (toolStripMenuItem2.Checked)
            {
                Properties.Settings.Default.Key = keyString;
                Properties.Settings.Default.Save();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double expiration = TOTP.GetExpirationTime();
            string nowText = "GAuth - " + expiration.ToString("00.0");
            label1.Invoke((MethodInvoker)delegate {
                if (Text.CompareTo(nowText) < 0 || totp == null)
                {
                    totp = TOTP.Compute(keyString);
                    label1.Text = totp;
                }
                if (10 <= expiration)
                {
                    label1.BackColor = GRC.Design.MakeColorFromHSV(120, 1, 1);
                }
                else if (9 <= expiration && expiration < 10)
                {
                    float hue = 60 + 60 * ((float)expiration - 9) / 1;
                    label1.BackColor = GRC.Design.MakeColorFromHSV(hue, 1, 1);
                }
                else if (5 <= expiration && expiration < 9)
                {
                    label1.BackColor = GRC.Design.MakeColorFromHSV(60, 1, 1);
                }
                else if (4 <= expiration && expiration < 5)
                {
                    float hue = 60 * ((float)expiration - 4) / 1;
                    label1.BackColor = GRC.Design.MakeColorFromHSV(hue, 1, 1);
                }
                else if (1 <= expiration && expiration < 4)
                {
                    label1.BackColor = GRC.Design.MakeColorFromHSV(0, 1, 1);
                }
                else if (expiration < 1)
                {
                    label1.BackColor = GRC.Design.MakeColorFromHSV(0, 1, (float)expiration);
                }
            });
            Text = nowText;
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(label1.Text);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip1.AutoClose = false;
            string s = toolStripTextBox1.Text;
            s = s.ToUpper().Replace(" ", "");
            toolStripTextBox1.Text = s;
            Regex rx = new Regex("^[" + Base32.base32Chars + "]{16}$");
            if (rx.IsMatch(s))
            {
                toolStripMenuItem2.CheckState = CheckState.Checked;
                keyString = s;
            }
            else
            {
                toolStripMenuItem2.CheckState = CheckState.Unchecked;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label1.Text);
            contextMenuStrip1.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            contextMenuStrip1_Opening(null, null);
            if (toolStripMenuItem2.Checked)
            {
                contextMenuStrip1.Close();
                totp = null;
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            toolStripMenuItem2.CheckState = CheckState.Indeterminate;
        }
    }
}
