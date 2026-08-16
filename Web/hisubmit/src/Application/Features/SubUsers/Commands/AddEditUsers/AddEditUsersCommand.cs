using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.SubUsers.Commands.AddEditUsers
{
    public class AddEditUsersCommand:IRequest<Result<string>>
    {
    }

}
