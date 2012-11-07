using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequentBreaker
{
    public class State
    {
        public DateTime PrevQuickBreakCompleted { get; set; }
        public DateTime PrevBreakCompleted { get; set; }
        public DateTime NextQuickBreak { get; set; }
        public DateTime NextBreak { get; set; }

        public TimeSpan LastIdleTime { get; set; }
        public bool NaturalBreak { get; set; }
        public bool NaturalQuickBreak { get; set; }

        public State()
        {
            PrevQuickBreakCompleted = DateTime.Now;
            PrevBreakCompleted = DateTime.Now;
        }

        public TimeSpan getIdleTime() {
            int idleTime = IdleTimeHelper.getIdleTime();
            return new TimeSpan(0, 0, idleTime/1000);
        }
        
    }


}
