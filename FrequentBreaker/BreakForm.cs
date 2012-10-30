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
    public partial class BreakForm : Form
    {
        MainForm parent;
        

        public BreakForm(MainForm parentForm)
        {
            InitializeComponent();

            // Place this window to the bottom right of the screen
            Rectangle r = Screen.GetWorkingArea(this);
            this.Location = new Point((r.Width + r.X) - this.Width, (r.Height + r.Y) - this.Height);
            
            this.parent = parentForm;
            
            progressBar.Maximum = parent.breakDuration / progressTimer.Interval;
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value == progressBar.Maximum)
            {
                parent.log("Break complete.");
                parent.startQBTimers();
                parent.startBTimers();
                progressTimer.Stop();
                this.Hide();
            }

            progressBar.PerformStep();
            parent.writeProgess(progressBar);
  
        }


        private void btnSkip_Click(object sender, EventArgs e)
        {
            parent.log("Break skipped.");
            progressTimer.Stop();
            parent.startQBTimers();
            parent.startBTimers();
            this.Hide();
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            parent.log("Breaking disabled from break.");
            progressTimer.Stop();
            parent.disable();
            this.Hide();
        }

        private void BreakForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // this cancels the close event.
        }

        public void attemptStartBreak()
        {
            parent.log("Attmpting to start break.");
            this.progressBar.Value = 0;
            parent.writeProgess(progressBar);
            this.Show();
            idleChecker.Start();
        }



        private void idleChecker_Tick(object sender, EventArgs e)
        {
            int idleTime = IdleTimeHelper.getIdleTime();
            if (idleTime > parent.idleBeforeBreak)
            {
                this.progressTimer.Start();
                idleChecker.Stop();
            }
        }
    }
}
