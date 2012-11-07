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
            double max = parent.breakLength.TotalMilliseconds / progressTimer.Interval;
            progressBar.Maximum = Convert.ToInt32(max);
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value == progressBar.Maximum)
            {
                parent.s.NextBreak = DateTime.Now + parent.timeBetweenBreaks;
                parent.log("Break complete.");
                progressTimer.Stop();
                this.Hide();
            }

            progressBar.PerformStep();
            parent.writeProgess(progressBar);
  
        }


        private void btnSkip_Click(object sender, EventArgs e)
        {
            parent.log("Break skipped.");
            parent.s.NextBreak = DateTime.Now + parent.timeBetweenBreaks;
            progressTimer.Stop();
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

        private void button1_Click(object sender, EventArgs e)
        {
            parent.log("Break postponed.");
            parent.s.NextBreak = DateTime.Now + parent.postponeLength;
            progressTimer.Stop();
            this.Hide();
        }
    }
}
