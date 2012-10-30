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
    public partial class QuickBreakForm : Form
    {
        MainForm parent;
        

        public QuickBreakForm(MainForm parentForm)
        {
            InitializeComponent();

            // Place this window to the bottom right of the screen
            Rectangle r = Screen.GetWorkingArea(this);
            this.Location = new Point((r.Width + r.X) - this.Width, (r.Height + r.Y) - this.Height);
            
            this.parent = parentForm;
            
            progressBar.Maximum = parent.quickBreakDuration / progressTimer.Interval;
        }


        // Perform quick break step and finish if time runs out
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value == progressBar.Maximum)
            {
                parent.log("Quick break complete.");
                parent.startQBTimers();
                progressTimer.Stop();
                this.Hide();
            }
            progressBar.PerformStep();
            parent.writeProgess(progressBar);
  
        }


        private void btnSkip_Click(object sender, EventArgs e)
        {
            parent.log("Quick break skipped.");
            progressTimer.Stop();
            parent.startQBTimers();
            this.Hide();
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            parent.log("Breaking disabled from quick break.");
            progressTimer.Stop();
            parent.disable();
            this.Hide();
        }

        private void QuickBreakForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // this cancels the close event.
        }

        public void attemptStartQuickBreak()
        {
            parent.log("Attmpting to start quick break.");
            this.progressBar.Value = 0;
            this.Show();
            qbIdleChecker.Start();
        }


        private void qbIdleChecker_Tick(object sender, EventArgs e)
        {
            int idleTime = IdleTimeHelper.getIdleTime();
            parent.log("Idle time:" + idleTime + ".");
            if (idleTime > parent.idleBeforeBreak)
            {
                this.progressTimer.Start();
                parent.writeProgess(progressBar);
                qbIdleChecker.Stop();
            }
            
        }
    }
}
