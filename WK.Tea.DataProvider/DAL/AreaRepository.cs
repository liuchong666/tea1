using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel.SqlModel;
using WK.Tea.DataProvider.IDAL;

namespace WK.Tea.DataProvider.DAL
{
    public class AreaRepository : BaseRepository<TeaDbContext, Area>, IArea
    {
    }
}
