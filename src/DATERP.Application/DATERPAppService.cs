using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace DATERP;

/* Inherit your application services from this class.
 */
public abstract class DATERPAppService : ApplicationService
{
    protected DATERPAppService()
    {
        // LocalizationResource = typeof(DATERPResource);
    }
}
