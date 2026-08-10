using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hisubmit.Hisubmit.Client.SharedModels.Features.Likes;

public class GetLikeCountRequest
{
    public int? FestivalId{ get; set; }
    public int? NewId { get; set; }
}