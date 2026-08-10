using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Content;

public class StaticPageAndFAQ : AuditableEntity<int>
{
    public string Title { get; set; }
    public string Link { get; set; }
    public string Content { get; set; }
    public bool IsEnable { get; set; }
    public FaqType FaqType { get; set; }
    public ContentType Type { get; set; }
}

public enum ContentType:byte
{
    StaticPage=0,
    Faq=1
}

public enum FaqType : byte
{
    General = 0,
    Festival = 1,
  Artist=2
}
