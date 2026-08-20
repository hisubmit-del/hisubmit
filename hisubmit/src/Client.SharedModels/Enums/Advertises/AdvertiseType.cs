using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums.Advertises
{
    public  enum AdvertiseType
    {
        [Display(Name = "Festival search sponsored card")]
        Banner,
        [Display(Name = "Site and email editorial feature")]
        ReportageBoth,
        [Display(Name = "Site editorial feature")]
        ReportageSite,
        [Display(Name = "Email newsletter feature")]
        ReportageEmail,
    }
}
