using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech.aerove.streamdeck.client.Events.SharedModels;

namespace tech.aerove.streamdeck.client.Events
{
    public class WillAppearEvent : ElgatoEvent, IActionEvent
    {
        public override ElgatoEventType Event { get; set; }
        public string Action { get; set; }
        public string Context { get; set; }
        public string Device { get; set; }
        public Payload Payload { get; set; }

        //added by StreamDeckAnalyzerMiddelware
        public string? ProfileName { get; set; }
        public string? FolderName { get; set; }
        public int? Page { get; set; }
        public string? PageId { get; set; }
        public string? FolderId { get; set; }
        public string? ProfileId { get; set; }
        public bool? CreatedNow { get; set; }

    }
}
