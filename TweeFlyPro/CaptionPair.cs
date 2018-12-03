using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweeFly
{
    [Serializable]
    public class CaptionPair
    {
        public string captionName { get; set; } = "";
        public string caption { get; set; } = "";

        public CaptionPair() { }

        public CaptionPair(string _function, string _keyword)
        {
            captionName = _function;
            caption = _keyword;
        }
    }
}
