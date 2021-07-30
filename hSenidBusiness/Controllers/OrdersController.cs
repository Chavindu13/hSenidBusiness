using hSenidBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hSenidBusiness.Controllers
{
    public class OrdersController : Controller
    {
        hSenidEntities db = new hSenidEntities();
        // GET: Orders
        public ActionResult Index()
        {
            List<Bill> OrdersList = db.Bills.ToList();
            var products = db.Products.ToList();
            ViewBag.ProductsList = new SelectList(products, "Name", "Name");
            return View(OrdersList);
        }

        public ActionResult SaveOrder(string discount, string vat, Detail[] orderDetails)
        {
            string result = "Error! Order Is Not Complete!";
            if (discount != null && vat != null && orderDetails != null)
            {
                float subTotal = 0, Total = 0;
                float OrderDiscount = float.Parse(discount);
                float OrderVAT = float.Parse(vat);

                Bill billModel = new Bill();
                billModel.Discount = OrderDiscount;
                billModel.Vat = OrderVAT;
                billModel.Date = DateTime.Now;

                foreach (var item in orderDetails)
                {
                    Detail detailModel = new Detail();
                    detailModel.Name = item.Name;
                    detailModel.Quantity = item.Quantity;
                    detailModel.Price = item.Price;
                    detailModel.Amount = item.Amount;
                    detailModel.Bill = billModel;
                    db.Details.Add(detailModel);
                    subTotal += item.Amount;
                }
                Total = subTotal * ((100 - OrderDiscount)/100) * ((100 + OrderVAT) / 100);
                billModel.Total = Total;
                billModel.SubTotal = subTotal;
                db.Bills.Add(billModel);
                db.SaveChanges();
                result = "Success! Order Is Complete!";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrice(string product)
        {
            var selectedProduct = db.Products.Where(x => x.Name == product)
                                .Select(x => new {
                                    Price = x.Price,
                                }).Single();
            float result = selectedProduct.Price;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}