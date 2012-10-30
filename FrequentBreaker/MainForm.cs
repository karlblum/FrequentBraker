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
        public int quickBreakDuration = 60 * 1000; // milliseconds
        private int quickBreakInterval = 12 * 60 * 1000; // milliseconds

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

        public void log(string s){
            textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + s + "\r\n");
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
            log("Starting main timers.");
            enabled = true;
            quickBreakTimer.Start();
            quickBreakResetTimer.Start();
            breakTimer.Start();
            breakResetTimer.Start();
        }

        // disable breaking
        public void disable()
        {
            log("Disabling main timers.");
            enabled = false;
            quickBreakTimer.Stop();
            quickBreakResetTimer.Stop();
            breakTimer.Stop();
            breakResetTimer.Stop();
        }


        // stop quick break timers
        public void stopQBTimers(){
            log("Stopping quick break timers.");
            quickBreakTimer.Stop();
            quickBreakResetTimer.Stop();
        }


        // start quick break timers
        public void startQBTimers()
        {
            if (enabled)
            {
                log("Starting quick break timers.");
                quickBreakTimer.Start();
                quickBreakResetTimer.Start();
            }
        }


        // stop break timers
        public void stopBTimers()
        {
            log("Stopping break timers.");
            breakTimer.Stop();
            breakResetTimer.Stop();
        }


        // start break timers
        public void startBTimers()
        {
            if (enabled)
            {
                log("Starting quick break timers.");
                breakTimer.Start();
                breakResetTimer.Start();
            }
        }

        private void quickBreakResetTimer_Tick(object sender, EventArgs e)
        {
            int idleTime = IdleTimeHelper.getIdleTime();
            if (idleTime > quickBreakDuration)
            {
                log("Quick break timer reset due to idling.");
                quickBreakTimer.Stop();
                quickBreakTimer.Start();
            }
        }

        private void breakResetTimer_Tick(object sender, EventArgs e)
        {
            int idleTime = IdleTimeHelper.getIdleTime();
            if (idleTime > breakDuration)
            {
                log("Break timer reset due to idling.");
                breakTimer.Stop();
                breakTimer.Start();
            }
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
    }
}
