using System;
using System.Collections.Generic;
using Taga.UserApp.Core.Model.Common.Enums;

namespace Taga.UserApp.Core.Model.Database
{
    public class Post
    {
        public virtual long Id { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual PostType PostType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual PostStatus Status { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] ImageBytes { get; set; }
        public virtual string VideoUrl { get; set; }
        public virtual string QuoteAuthor { get; set; }
        public virtual string QuoteText { get; set; }

        public virtual Category Category { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
