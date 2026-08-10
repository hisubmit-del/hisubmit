using HiSubmit.Application.Specifications.Base;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Domain.Entities.Content;

namespace HiSubmit.Application.Specifications.Contents;

public class GetAllNewFilterSpecification:HeroSpecification<New>
{
    public GetAllNewFilterSpecification(string searchString,bool? isEnable,int? festivalId,bool getFestivalNews )
    {
        Criteria = n =>(string.IsNullOrWhiteSpace(searchString)|| n.Title.Contains(searchString)) &&
                         (isEnable==null || n.IsEnable==isEnable) &&
                         (!getFestivalNews || n.FestivalId==festivalId) 
                         ;
    }



}

public class StaticPageAndFaqFilterSpecification : HeroSpecification<StaticPageAndFAQ>
{
    public StaticPageAndFaqFilterSpecification(GetAllStaticPageRequest filter)
    {
        Criteria = st => (string.IsNullOrWhiteSpace(filter.SearchString) || st.Title.Contains(filter.SearchString)) &&
                         st.Type == (ContentType)filter.Type;
    }
}

