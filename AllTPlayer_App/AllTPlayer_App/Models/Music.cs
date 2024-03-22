using System;
using System.Collections.Generic;

using System.Text;
using Xamarin.Forms;

namespace AllTPlayer_App.Models
{
    public class Music
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Url { get; set; }
        public ImageSource CoverImage { get; set; }
        public bool IsRecent { get; set; }
        
    }
}
