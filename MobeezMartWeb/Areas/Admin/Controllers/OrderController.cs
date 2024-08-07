using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using MobeezMart.Models.ViewModels;
using MobeezMart.Utility;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace MobeezMartWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM orderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
        public IActionResult Details( int orderId)
        {
			orderVM = new OrderVM()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),

			};
            return View(orderVM);
        }
		[ActionName("Details")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Details_PAY_NOW()
		{
			orderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderVM.OrderHeader.Id, includeProperties: "Product");


			//stripe settings 
			var domain = "https://localhost:44343/";
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>
				{
				  "card",
				},
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={orderVM.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
			};

			foreach (var item in orderVM.OrderDetail)
			{

				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.DeviceName
						},

					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionLineItem);

			}

			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStripePaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}
		public IActionResult PaymentConfirmation(int orderHeaderid)
		{

			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderid);
			if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				//check the stripe status
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			return View(orderHeaderid);
		}
		[HttpPost]
		[Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail()
		{
			var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked:false);
			orderHEaderFromDb.Name= orderVM.OrderHeader.Name;
			orderHEaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
			orderHEaderFromDb.StreetAddress= orderVM.OrderHeader.StreetAddress;	
			orderHEaderFromDb.City= orderVM.OrderHeader.City;
			orderHEaderFromDb.State= orderVM.OrderHeader.State;
			orderHEaderFromDb.PostalCode= orderVM.OrderHeader.PostalCode;
			if (orderVM.OrderHeader.Carrier != null)
			{
				orderHEaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
			}
			if (orderVM.OrderHeader.TrackingNumber != null)
			{
				orderHEaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderHEaderFromDb);
			_unitOfWork.Save();
			TempData["success"] = "Order Details Updated Successfully!";

			return RedirectToAction("Details","order",new { orderId = orderHEaderFromDb.Id});
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["success"] = "Order Status Updated Successfully!";
			return RedirectToAction("Details", "order", new { orderId = orderVM.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked: false);
			orderHeader.TrackingNumber= orderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = orderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate=DateTime.Now;

			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["success"] = "Order Shipped Successfully!";
			return RedirectToAction("Details", "order", new { orderId = orderVM.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked: false);
			if (orderHeader.PaymentStatus==SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,

				};
				var service = new RefundService();
				Refund refund = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

			}
			_unitOfWork.Save();

			TempData["success"] = "Order Cancelled Successfully!";
			return RedirectToAction("Details", "order", new { orderId = orderVM.OrderHeader.Id });
		}
		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;

			if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
			{
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            }
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==claim.Value,includeProperties: "ApplicationUser");


            }
            switch (status)
			{
                case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
				case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }

            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
