using System.Linq;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Client.SharedModels.Wrapper;

namespace ClientComponents.Pages.Public
{
    public partial class FAQ
    {
        [Inject] 
        private IContentManager ContentManager { get; set;}

        private PaginatedResult<GetAllStaticPageResponse> _faqResponse = new([]);


        private bool _loaded;
        private PersistingComponentStateSubscription _subscription;

        protected override async Task OnInitializedAsync()
        {
            _subscription = ApplicationState.RegisterOnPersisting(PersistContent);
            await LoadStaticPages();
            var g = _faqResponse.Data.FirstOrDefault(p=>p.FaqType==FaqType.General);
            if (g != null)
                g.IsSelected = true;
            var f = _faqResponse.Data.FirstOrDefault(p => p.FaqType==FaqType.Festival);
            if (f != null)
                f.IsSelected = true;
            var a = _faqResponse.Data.FirstOrDefault(p => p.FaqType==FaqType.Artist);
            if (a != null)
                a.IsSelected = true;
            _loaded = true;
            await base.OnInitializedAsync();
        }

        private Task PersistContent()
        {
            ApplicationState.PersistAsJson("faq", _faqResponse);
            return Task.CompletedTask;
        }

        private async Task LoadStaticPages()
        {
            if (ApplicationState.TryTakeFromJson
                    <PaginatedResult<GetAllStaticPageResponse>>
                    ("faq", out var stored))
            {
                _faqResponse = stored;
            }
            else
            {
                var response = await ContentManager.GetAllFAQ(new GetAllStaticPageRequest()
                {
                 GetAllData   = true,
                 Type = ContentType.Faq,
                 IsEnable = true
                });
                if (response.Succeeded)
                    _faqResponse = response;
                else
                    foreach (var message in response.Messages)
                        _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }

        private void SetSelect(GetAllStaticPageResponse f)
        {
            foreach (var v in _faqResponse.Data.Where(p=>p.FaqType==f.FaqType))
            {
             v.IsSelected=false;
            }
            f.IsSelected = true;
        }
    }
}
