using System.Collections.Generic;

namespace NewEraFlowerStore.Data
{
    // any changes in this class require an update to the relevant code in the pages for orders
    public class OrderStatusListItem
    {
        public string DisplayName { get; set; }

        public int ID { get; set; }

        public string Description { get; set; }

        public List<OrderStatusListItem> GetOrderStatusList()
        {
            List<OrderStatusListItem> orderStatusList = new List<OrderStatusListItem>
            {
                new OrderStatusListItem()
                {
                    DisplayName = "All",
                    ID = 0,
                    Description = null
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Pending delivery info",
                    ID = 1,
                    Description = "Delivery info has to be confirmed in 15 minutes after creating an order. For a registered customer, delivery info cannot be modified after payment." // for administrators, delivery info cannot be modified after an order is pulled and packaged
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Failed",
                    ID = 2,
                    Description = "An order is failed due to a timeout, insufficient stocks, any bouquets off the shelves, or other reasons. The store staff may contact with the customer to state a detailed reason and work out a solution."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Cancelled",
                    ID = 3,
                    Description = "The customer has cancelled an order. Please notice that an order cannot be cancelled after payment. After delivery, the customer may refund an order which cannot be cancelled."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Pending payment",
                    ID = 4,
                    Description = "Payment has to be confirmed in 30 minutes after creating an order."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Awaiting fulfillment",
                    ID = 5,
                    Description = "An order is being pulled and packaged."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Awaiting delivery",
                    ID = 6,
                    Description = "An order has been pulled and packaged, and is awaiting collection from a delivery service provider."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Being delivered",
                    ID = 7,
                    Description = "An order is being delivered."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Delivered",
                    ID = 8,
                    Description = "An order has been delivered and all bouquets have been received by the consumer, but the receipt has not been confirmed."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Completed",
                    ID = 9,
                    Description = "The receipt has been confirmed, so an order has been completed. Please notice that due to particularities of bouquets, an order will be automatically completed if the consumer has not asked for a refund in 24 hours after delivery."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Being refunded",
                    ID = 10,
                    Description = "An order is failed after payment, or the consumer has asked for a refund after delivery. Please notice that once an order is completed, it cannot be refunded."
                },
                new OrderStatusListItem()
                {
                    DisplayName = "Refund request processed",
                    ID = 11,
                    Description = "After a discussion with the store staff, an order has been refunded and completed, or the refund request has been rejected and an order has been completed."
                }
            };
            return orderStatusList;
        } // end method GetOrderStatusList

        public bool IsValidId(int ID)
        {
            switch (ID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    return true;

                default:
                    return false;
            } // end switch-case
        } // end method IsValidId
    } // end class OrderStatusListItem
} // end namespace NewEraFlowerStore.Data