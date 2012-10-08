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
        public int quickBreakDuration = 1 * 1000; // milliseconds
        private int quickBreakInterval = 4 * 1000; // milliseconds
        public int breakDuration = 3 * 1000; // milliseconds
        private int breakInterval = 10 * 1000;// milliseconds

        public int idleBeforeBreak = 2 * 1000; // milliseconds


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
            pause();
       }

        private void breakTimer_Tick(object sender, EventArgs e)
        {
            breakForm.attemptStartBreak();
            pause();
        }

        public void log(string s){
            textBox1.AppendText(s+"\n");
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
            this.Close();
        }

        private void showMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        public void enable()
        {
            log("Starting main timers.");
            enabled = true;
            quickBreakTimer.Start();
            quickBreakResetTimer.Start();
            breakTimer.Start();
        }

        public void disable()
        {
            log("Disabling main timers.");
            enabled = false;
            quickBreakTimer.Stop();
            quickBreakResetTimer.Stop();
            breakTimer.Stop();
        }

        public void pause(){
            log("Pausing main timers.");
            quickBreakTimer.Enabled = false;
            quickBreakResetTimer.Enabled = false;
            breakTimer.Enabled = false;
        }

        public void resume()
        {
            if (enabled)
            {
                log("Resuming main timers.");
                quickBreakTimer.Enabled = true;
                quickBreakResetTimer.Enabled = true;
                breakTimer.Enabled = true;
            }
        }


    }
}
