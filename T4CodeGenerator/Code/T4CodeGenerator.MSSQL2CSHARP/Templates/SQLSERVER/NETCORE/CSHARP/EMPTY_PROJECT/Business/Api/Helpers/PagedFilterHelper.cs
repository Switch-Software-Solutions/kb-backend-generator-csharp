using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Helpers
{
    public static class PagedFilterHelper
    {
        public static bool AreValidatePagedFilerValues(int? page, int? pageSize, string? order, string? sort, out bool paged) 
        {
            bool result = false;
            paged = false;

            if (page == null && pageSize == null && order == null && sort == null)
            {
                result = true;
                paged = false;
            }
            else if (page >= 0 && pageSize >= 1 && order != null && order != null) 
            {
                result = true;
                paged = true;
            }

            return result;
        }
    }
}
