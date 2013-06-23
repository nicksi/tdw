using System;
using Microsoft.SPOT;

namespace AgentWatchFace1
{
        
    enum ToDoEventTypes
    {
        EVENT_CALL = 0,
        EVENT_MEET = 1,
        EVENT_ALARM = 2,
        EVENT_WARN = 3,
        EVENT_LOVE = 4,
        EVENT_HATE = 5
    }

    class ToDoEvent
    {
        public DateTime DueDate { get; set; }
        public ToDoEventTypes Type { get; set; }
        public string Label { get; set; }
    }
}
