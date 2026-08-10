using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Specifications.Payments
{
    public class GetOpenCartUserSpecification:HeroSpecification<Cart>
    {
        public GetOpenCartUserSpecification(string userId)
        {
            Criteria = (cart) => cart.UserId == userId && !cart.Paid;
        }
    }
}

