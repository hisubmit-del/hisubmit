
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Client.Infrastructure.Managers.JudgingProjects;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Festival.JudgingProjects
{
    public partial class AddProjectsToRefree
    {
        [Parameter]
        public int FestivalId { get; set; }

        [Parameter]
        public string RefereeId { get; set; }

        [Inject]
        public IProjectJudgingManager ProjectJudgingManager { get; set; }
        [Inject]
        public IProjectManager ProjectManager { get; set; }
        [Inject]
        public ISubmitManager SubmitManager { get; set; }

        public List<GetAllSubmitsResponse> Submits { get; set; }

        private bool hover = true;
        private HashSet<GetAllSubmitsResponse> selectedSubmits = new HashSet<GetAllSubmitsResponse>();

        //server load data and pagination
        public List<GetAllSubmitsResponse> _pagedDate { get; set; }
        private MudTable<GetAllSubmitsResponse> _table;
        private GetAllSubmitsRequest _advancedSearch { get; set; } = new GetAllSubmitsRequest();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        private string CurrentUserId { get; set; }
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        private bool _openSearchForm = false;
        private bool IsAdvancedSearch = false;
        private string _displaySearchFrom = "d-none";
        private bool _loaded;

        
        private  bool _processing { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
          // await LoadSelectedProjects();
        }

        private async Task<TableData<GetAllSubmitsResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            var query = new GetAllSubmitsRequest();
            if (IsAdvancedSearch)
            {
                query = _advancedSearch;
            }
            else
            {
                query.SearchString = _searchString;
            }
            await LoadData(state.Page, state.PageSize, state, query);
            return new TableData<GetAllSubmitsResponse> { TotalItems = _totalItems, Items = _pagedDate };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllSubmitsRequest request)
        {

            request.PageSize = pageSize;
            request.PageNumber = pageNumber + 1;
            request.FestivalId = FestivalId;
            var response = await SubmitManager.GetAll(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                var data = response.Data;
                var loadedData = data.Where(element =>
                {
                    if (string.IsNullOrWhiteSpace(request.SearchString))
                        return true;
                    if (element.ProjectTitle != null && element.ProjectTitle.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (element.FestivalName != null && element.FestivalName.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                    return false;
                });
                switch (state.SortLabel)
                {
                    case "SubmitProjectTitle":
                        loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.ProjectTitle);
                        break;
                    case "SubmitDateFrom":
                        loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.SubmitDate);
                        break;
                }
                data = loadedData.ToList();
                _pagedDate = data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            await LoadSelectedProjects();
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }


        private async Task AddToReferee()
        {
            _processing = true;
            var submitsId = selectedSubmits.Select(p => p.Id).ToList();
          
            var response = await ProjectJudgingManager.AddJudging(new AddEditProjectJudgingCommand(submitsId,new List<string> { RefereeId},FestivalId,true)); 
            if (response.Succeeded)
            {
                _snackBar.Add(Localizer["Successfully added to Judge"], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _processing = false;
        }

        private async Task LoadSelectedProjects()
        {
            var response = await ProjectJudgingManager.GetAll(new GetAllProjectJudgingQuery()
            {
                UserId = RefereeId,
                FestivalId=FestivalId
            });
            if (response.Succeeded)
            {
                selectedSubmits = _pagedDate
                    .Where(submit => response.Data.Any(projectjudging => projectjudging.SubmitId == submit.Id))
                    .ToHashSet();
                StateHasChanged();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message,Severity.Error);
                }
            }
        }
    }
}
