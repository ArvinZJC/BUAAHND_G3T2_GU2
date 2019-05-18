// csharp file that contains actions of the FAQ page

#region Using Directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore.Pages.Help
{
    /// <summary>
    /// Extending from the class <see cref="PageModel"/>, the class <see cref="FaqModel"/> contains actions of the FAQ page.
    /// </summary>
    public class FaqModel : PageModel
    {
        private readonly OrderStatusListItem _orderStatusListItem;

        public FaqModel()
        {
            _orderStatusListItem = new OrderStatusListItem();
            OrderStatusList = _orderStatusListItem.GetOrderStatusList();
        } // end constructor FaqModel

        /// <summary>
        /// An order status form.
        /// </summary>
        public List<object> OrderStatusForm { get; set; }
        /// <summary>
        /// An order status list.
        /// </summary>
        private List<OrderStatusListItem> OrderStatusList { get; }
        /// <summary>
        /// A status message decorated with <see cref="TempDataAttribute"/>.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            OrderStatusForm = new List<object>();

            foreach (var item in OrderStatusList)
                if (item.ID != 0)
                    OrderStatusForm.Add(new
                    {
                        item.DisplayName,
                        item.Description
                    });
        } // end method OnGet
    } // end class FaqModel
} // end namespace NewEraFlowerStore.Pages.Help