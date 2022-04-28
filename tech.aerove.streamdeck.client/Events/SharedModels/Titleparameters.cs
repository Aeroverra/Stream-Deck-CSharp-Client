﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech.Aerove.StreamDeck.Client.Events.SharedModels
{
    public class Titleparameters
    {
        public string fontFamily { get; set; }
        public int fontSize { get; set; }
        public string fontStyle { get; set; }
        public bool fontUnderline { get; set; }
        public bool showTitle { get; set; }
        public string titleAlignment { get; set; }
        public string titleColor { get; set; }
    }
}
