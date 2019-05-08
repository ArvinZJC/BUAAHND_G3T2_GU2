#region Using Directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    public class FaqModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;

        public FaqModel()
        {
            _orderStatusListItem = new OrderStatusListItem();
            OrderStatusList = _orderStatusListItem.GetOrderStatusList();
        } // end constructor FaqModel

        public List<object> OrderStatusForm { get; set; }

        private List<OrderStatusListItem> OrderStatusList { get; }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            OrderStatusForm = new List<object>();

            foreach (var item in OrderStatusList)
            {
                if (item.ID != 0)
                {
                    OrderStatusForm.Add(new
                    {
                        item.DisplayName,
                        item.Description
                    });
                }
            }
        } // end method OnGet
    } // end class FaqModel
} // end namespace NewEraFlowerStore.Pages.Help