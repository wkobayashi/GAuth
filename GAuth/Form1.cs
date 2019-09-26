using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GAuth
{
    public partial class Form1 : Form
    {

        private readonly string path;
        private Json json = null;
        private string keyString = null;
        private string totp = null;

        public Form1()
        {
            InitializeComponent();
            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GAuth\\GAuth.json";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            json = Setting.readSetting(path);
            keyString = (json.secret != string.Empty) ? Cryptograph.Decrypt(json.secret) : "MFRGGZDFGAYTEMZU"; // abcde01234
            toolStripMenuItem2_Click(null, null);
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

        private void label1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label1.Text);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string s = toolStripTextBox1.Text;
            s = s.ToUpper().Replace(" ", "");
            toolStripTextBox1.Text = s;
            Regex rx = new Regex("^[" + Base32.base32Chars + "]{16}$");
            if (rx.IsMatch(s))
            {
                toolStripMenuItem1.CheckState = CheckState.Checked;
                keyString = s;
                totp = null;
                json.secret = Cryptograph.Encrypt(keyString);
                Setting.writeSetting(path, json);
            }
            else
            {
                toolStripMenuItem1.CheckState = CheckState.Unchecked;
                toolStripTextBox1.BackColor = GRC.Design.MakeColorFromHSV(0, 1, 1);
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            toolStripMenuItem1.CheckState = CheckState.Unchecked;
            toolStripTextBox1.BackColor = SystemColors.Window;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItem1.CheckState == CheckState.Unchecked)
            {
                toolStripTextBox1.Text = keyString;
                toolStripMenuItem1.CheckState = CheckState.Checked;
            }
            contextMenuStrip1.Close();
        }
    }
}
