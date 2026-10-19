namespace HeistPlanner.Models
{
    //-------one line in the story of the night, for example at 1:06am Ana Ruiz loops the camera feed
    public class TimelineEvent : IComparable<TimelineEvent>
    {
        private DateTime time;
        private string text;
        private bool isBad;     //---true for things going wrong, shown in red

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public bool IsBad
        {
            get { return isBad; }
            set { isBad = value; }
        }

        public TimelineEvent()
        {
            time = DateTime.Today;
            text = "";
            isBad = false;
        }

        public TimelineEvent(DateTime time, string text, bool isBad)
        {
            this.time = time;
            this.text = text;
            this.isBad = isBad;
        }

        //--override methods
        public override string ToString()
        {
            return $"{time:h:mmtt} {text}";
        }

        //---earliest first so the timeline reads in order
        public int CompareTo(TimelineEvent? other)
        {
            if (other is null) return 1;
            return time.CompareTo(other.time);
        }
    }
}