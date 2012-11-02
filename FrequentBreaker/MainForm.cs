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
        private bool enabled = false;
        private bool naturalQuickBreak = false;
        private DateTime naturalQuickBreakStarted;
        private bool naturalBreak = false;
        private DateTime naturalBreakStarted;

        public int quickBreakDuration = 60 * 1000; // milliseconds
        private int quickBreakInterval = 15 * 60 * 1000; // milliseconds

        public int breakDuration = 10 * 60 * 1000; // milliseconds
        private int breakInterval = 55 * 60 * 1000;// milliseconds

        public int idleBeforeBreak = 5 * 1000; // milliseconds


        QuickBreakForm quickBreakForm;
        BreakForm breakForm;

        public MainForm()
        {
            InitializeComponent();
            log("Initializing.");
            quickBreakForm = new QuickBreakForm(this);
            quickBreakTimer.Interval = quickBreakInterval;

            breakForm = new BreakForm(this);
            breakTimer.Interval = breakInterval;


            enable();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void quickBreakTimer_Tick(object sender, EventArgs e)
        {
            quickBreakForm.attemptStartQuickBreak();
            stopQBTimers();
        }

        private void breakTimer_Tick(object sender, EventArgs e)
        {
            breakForm.attemptStartBreak();
            stopQBTimers();
            stopBTimers();
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
            log("Enabling system.");
            enabled = true;
            quickBreakTimer.Start();
            naturalBreakTimer.Start();
            breakTimer.Start();
            naturalBreakTimer.Start();
        }

        // disable breaking
        public void disable()
        {
            log("Disabling system.");
            enabled = false;
            quickBreakTimer.Stop();
            naturalBreakTimer.Stop();
            breakTimer.Stop();
            naturalBreakTimer.Stop();
        }


        // stop quick break timers
        public void stopQBTimers()
        {
            log("Stopping quick break timers.");
            quickBreakTimer.Stop();
            naturalBreakTimer.Stop();
        }


        // start quick break timers
        public void startQBTimers()
        {
            if (enabled)
            {
                log("Starting quick break timers.");
                quickBreakTimer.Start();
                naturalBreakTimer.Start();
            }
        }


        // stop break timers
        public void stopBTimers()
        {
            log("Stopping break timers.");
            breakTimer.Stop();
            naturalBreakTimer.Stop();
        }


        // start break timers
        public void startBTimers()
        {
            if (enabled)
            {
                log("Starting quick break timers.");
                breakTimer.Start();
                naturalBreakTimer.Start();
            }
        }

        private void naturalBreakTimer_Tick(object sender, EventArgs e)
        {
            // current idle time
            int idleTime = IdleTimeHelper.getIdleTime();

            

            /* Check if we have a natural quick break. 
             * If we have a natural quick break, then we have to stop the quick break timer, 
             * because idle time already exceeds the quick break time. 
             * This does not affect the break timer. 
             */
            if (idleTime >= quickBreakDuration && !naturalQuickBreak && !naturalBreak)
            {
                log("Natural break started.");
                naturalQuickBreak = true;
                naturalQuickBreakStarted = DateTime.Now;
                log("Stoping quick break timer.");
                quickBreakTimer.Stop();
            }
            // if natural quick break has ended
            else if (idleTime < quickBreakDuration && naturalQuickBreak)
            {
                double d = (DateTime.Now - naturalQuickBreakStarted).TotalMinutes + quickBreakDuration / (60 * 1000);
                d = Math.Round(d, 0);
                log("Natural quick break (" + d + " min).");
                naturalQuickBreak = false;
                log("Starting quick break timer.");
                quickBreakTimer.Start();
            }

            /* Check if we have a natural break. 
             * If we have a natural break, then we have to stop the break timer. 
             * The quick break timer has already stopped because we assume 
             * that the quick break is always shorter than this break. */
            if (idleTime >= breakDuration && !naturalBreak)
            {
                naturalBreak = true;
                naturalQuickBreak = false; // We now have a natural break and we don't care about natural quick break.
                naturalBreakStarted = DateTime.Now;
                log("Stoping break timer.");
                breakTimer.Stop();
            }
            // if natural break has ended, then start both break timers
            else if (idleTime < breakDuration && naturalBreak)
            {
                double d2 = (DateTime.Now - naturalBreakStarted).TotalMinutes + breakDuration / (60 * 1000);
                d2 = Math.Round(d2, 0);
                log("Natural break (" + d2 + " min).");
                naturalBreak = false;

                log("Starting break timer.");
                breakTimer.Start();
                log("Starting quick break timer.");
                quickBreakTimer.Start();
            }



            txtbIdleTime.Text = (idleTime / 1000).ToString();
            txtbBreakTimerStatus.Text = breakTimer.Enabled.ToString();
            txtbQuickBreakTimerStatus.Text = quickBreakTimer.Enabled.ToString();
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
    }
}
