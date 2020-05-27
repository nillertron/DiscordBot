using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model
{
    public class ResponseBody
    {
        public List<Equipped_Items> equipped_items { get; set; }
        public string Class { get; set; }
        public Gear gear { get; set; }
        public MythicPlusScores mythic_plus_scores { get; set; }
    }
}
