using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech.aerove.streamdeck.client.SDAnalyzer.ManifestModels
{
    internal class MState
    {
        //Manifest Values
        public string FFamily { get; set; }
        public string FSize { get; set; }
        public string FStyle { get; set; }
        public string FUnderline { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string TitleAlignment { get; set; }
        public string TitleColor { get; set; }
        public string TitleShow { get; set; }

        //Custom Values
        public string ImageData { get; set; }
        public ImageSource ImageSource { get; set; } = ImageSource.Unknown;
    }
}
