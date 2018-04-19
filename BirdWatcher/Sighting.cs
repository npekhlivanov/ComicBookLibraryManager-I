using System;

namespace SimpleBirdWatcher
{
    public class Sighting
    {
        public Bird Bird { get; set; }
        public DateTime SightingDate { get; set; }
        public string ObserverFirstName { get; set; }
        public string ObserverLastName { get; set; }
        public Place Place { get; set; }
    }
}
