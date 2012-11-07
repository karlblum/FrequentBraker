using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrequentBreaker
{
    public partial class MainForm : Form
    {


        public State s = new State();

        // new system parameters
        public TimeSpan timeBetweenQuickBreaks = new TimeSpan(0, 10, 0); //hh,mm,ss
        public TimeSpan timeBetweenBreaks = new TimeSpan(0, 50, 0); //hh,mm,ss
        public TimeSpan quickBreakLength = new TimeSpan(0, 0, 50); //hh,mm,ss
        public TimeSpan breakLength = new TimeSpan(0, 9, 0); //hh,mm,ss
        public TimeSpan postponeLength = new TimeSpan(0, 5, 0); //hh,mm,ss

        /*
        public TimeSpan timeBetweenQuickBreaks = new TimeSpan(0, 0, 10); //hh,mm,ss
        public TimeSpan timeBetweenBreaks = new TimeSpan(0, 0, 40); //hh,mm,ss
        public TimeSpan quickBreakLength = new TimeSpan(0, 0, 5); //hh,mm,ss
        public TimeSpan breakLength = new TimeSpan(0, 0, 8); //hh,mm,ss
        */

        public int idleBeforeBreak = 2 * 1000; // milliseconds

        QuickBreakForm quickBreakForm;
        BreakForm breakForm;

        public MainForm()
        {
            InitializeComponent();
            log("Initializing.");

            quickBreakForm = new QuickBreakForm(this);
            breakForm = new BreakForm(this);
            enable();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }


        public void log(string s)
        {
            textBox1.AppendText("[" + DateTime.Now.ToString("yyyy.MM.dd - HH:mm:ss") + "] " + s + "\r\n");
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disableToolStripMenuItem.Text.Equals("Disable"))
            {
                disable();
                disableToolStripMenuItem.Text = "Enable";
            }
            else
            {
                enable();
                disableToolStripMenuItem.Text = "Disable";
            }


        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        // enable breaking
        public void enable()
        {
            s.NextBreak = DateTime.Now + timeBetweenBreaks;
            s.NextQuickBreak = DateTime.Now + timeBetweenQuickBreaks;
        }

        // disable breaking
        public void disable()
        {

        }

        internal void writeProgess(ProgressBar progressBar)
        {
            progressBar.Refresh();
            using (Graphics gr = progressBar.CreateGraphics())
            {
                String s = progressBar.Value.ToString() + "/" + progressBar.Maximum.ToString();
                gr.DrawString(s,
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    new PointF(progressBar.Width / 2 - (gr.MeasureString(s,
                        SystemFonts.DefaultFont).Width / 2.0F),
                    progressBar.Height / 2 - (gr.MeasureString(s,
                        SystemFonts.DefaultFont).Height / 2.0F)));
            }
            //progressBar.Refresh();
            //progressBar.CreateGraphics().DrawString(progressBar.Value.ToString() + "/" +  progressBar.Maximum.ToString(), new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar.Width / 2 - 10, progressBar.Height / 2 - 7));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true; // this cancels the close event.
                Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            txtbIdleTime.Text = s.getIdleTime().Hours + ":" + s.getIdleTime().Minutes + ":" + s.getIdleTime().Seconds;
            txtbNextBreak.Text = s.NextBreak.ToLongTimeString();
            txtbNextQuickBreak.Text = s.NextQuickBreak.ToLongTimeString();
            txtbCurrentTime.Text = DateTime.Now.ToLongTimeString();


            if (!s.NaturalBreak && s.getIdleTime().TotalSeconds > breakLength.TotalSeconds)
            {
                log("natural break");
                s.NaturalBreak = true;
                s.LastIdleTime = s.getIdleTime();
            }
            else if (!s.NaturalQuickBreak && s.getIdleTime().TotalSeconds > quickBreakLength.TotalSeconds)
            {
                log("natural quick break");
                s.NaturalQuickBreak = true;
                s.LastIdleTime = s.getIdleTime();
            }
            // if natural breaking has ended then reset next break times
            else if (s.NaturalBreak && s.LastIdleTime.TotalSeconds > s.getIdleTime().TotalSeconds)
            {
                log("natural break ended");
                s.NaturalQuickBreak = false;
                s.NaturalBreak = false;
                s.NextQuickBreak = DateTime.Now + timeBetweenQuickBreaks;
                s.NextBreak = DateTime.Now + timeBetweenBreaks;
            }
            else if (s.NaturalQuickBreak && s.LastIdleTime.TotalSeconds > s.getIdleTime().TotalSeconds)
            {
                log("natural quick break ended");
                s.NaturalQuickBreak = false;
                s.NextQuickBreak = DateTime.Now + timeBetweenQuickBreaks;
            }
            else if (s.NaturalQuickBreak || s.NaturalBreak)
            {

            } 
            // else check if we should have a break
            else if (s.NextBreak <= DateTime.Now)
            {
                log("should start b");
                breakForm.attemptStartBreak();
                s.NextBreak = DateTime.MaxValue;
            }
            else if (s.NextQuickBreak <= DateTime.Now)
            {
                log("should start new qb");
                quickBreakForm.attemptStartQuickBreak();
                s.NextQuickBreak = DateTime.MaxValue;
            }
        }
    }
}
