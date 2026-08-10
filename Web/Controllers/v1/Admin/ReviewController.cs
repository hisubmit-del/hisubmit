using System.Threading.Tasks;
using HiSubmit.Application.Features.Reviews.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin
{

    public class ReviewController : BaseAdminController<ReviewController>
    {
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(GetAllReviewQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

    }
}
