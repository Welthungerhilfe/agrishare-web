using Agrishare.API;
using Agrishare.API.Models;
using Agrishare.Core;
using System.Linq;
using System.Web.Http;
using Entities = Agrishare.Core.Entities;

namespace Agrishare.API.Controllers.CMS
{
    [@Authorize(Roles="Administrator")]
    [RoutePrefix("cms")]
    public class CmsJournalsController : BaseApiController
    {
        [Route("journals/list")]
        [AcceptVerbs("GET")]
        public object List(int PageIndex, int PageSize, string Query = "")
        {
            var recordCount = Entities.Journal.Count();
            var list = Entities.Journal.List(PageIndex: PageIndex, PageSize: PageSize);
            var title = "Ledger";

            var balance = Entities.Journal.BalanceAt(list.First()?.Id ?? 0);
            foreach (var item in list)
            {
                item.Balance = balance;
                balance -= item.Amount;
            }               

            var data = new
            {
                Count = recordCount,
                Sort = Entities.Booking.DefaultSort,
                List = list.Select(e => e.Json()),
                Title = title
            };

            return Success(data);
        }

        [Route("journals/reconcile/find")]
        [AcceptVerbs("GET")]
        public object Find(int Id = 0)
        {
            var data = new
            {
                Entity = new ReconcileModel(),
            };

            return Success(data);
        }

        [Route("journals/reconcile/save")]
        [AcceptVerbs("POST")]
        public object Save(ReconcileModel Model)
        {
            if (!ModelState.IsValid)
                return Error(ModelState);

            // TODO process 
            var import = new Entities.JournalImport();
            import.ExcelFile = Model.ExcelFile;
            var success = import.Process(out var feedback);

            if (success)
                return Success(feedback);

            return Error(feedback);
        }
    }

}
