using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer.DataModel
{
    public partial class CartoonStory
    {
        public long Id { get; set; }
        public long CartoonChapterId { get; set; }
        public int StoryNum { get; set; }
        public string Title { get; set; }
    }
}
