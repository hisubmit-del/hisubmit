using System.Collections.ObjectModel;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;

namespace Hisubmit.Client.SharedModels.Features.Tickets.Commands.AddEditTickets;

public class AddEditTicketsCommand
{
    public  int Id { get; set; }
    public  int FestivalId { get; set; }
    public string Title { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public bool AddManagerPercentage { get; set; }
    public int Cost { get; set; }

    public List<AddEditSubmissionQuestionCommand> SubmissionQuestions { get; set; }


    public  DateTime? EventDate { get; set; }
    
    //Capacity
    public  int Capacity { get; set; }

    public string Description { get; set; }
    
    
    public int VenueId { get; set; }

    public List<int> ShowHallId { get; set; }

    public IReadOnlyCollection<int> ShowTimesId { get; set; }

    public AddEditTicketsCommand()
    {
        SubmissionQuestions = new List<AddEditSubmissionQuestionCommand>();
        ShowHallId = new List<int>();
        ShowTimesId = new ReadOnlyCollection<int>(new List<int>());
    }
}

