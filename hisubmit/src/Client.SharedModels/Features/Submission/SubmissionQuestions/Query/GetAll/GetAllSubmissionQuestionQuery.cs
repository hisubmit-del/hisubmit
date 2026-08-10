namespace Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;

public class GetAllSubmissionQuestionQuery 
{
    public int? FestivalId { get; set; }
    public string CategoriesIdString { get; set; }
    public int? JudgingId { get; set; }
    public bool IncludeAnswer { get; set; }

    public GetAllSubmissionQuestionQuery()
    {
           
    }

    public List<int> GetRealCategories()
    {
        return CategoriesIdString.Split(',').Select(p => int.Parse(p)).ToList();
    }

}