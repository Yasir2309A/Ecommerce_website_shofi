using Ecommerce_website.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce_website.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext context;

        public AdminController(MyDbContext context,IWebHostEnvironment env)
        {
            this.context = context;
            Env = env;
        }

        public IWebHostEnvironment Env { get; }

        public IActionResult Index()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                return View("Index");
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }

        //admin Login Start

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string user_email,string user_password)
        {
            var admin = context.admins.Where(x => x.admin_email == user_email && x.admin_password == user_password).FirstOrDefault();
            if (admin != null)
            {
                HttpContext.Session.SetString("mysession", admin.admin_id.ToString());
                HttpContext.Session.SetString("adminName", admin.admin_name);
                HttpContext.Session.SetString("admin_image", admin.admin_image);
                HttpContext.Session.SetString("email", admin.admin_email);

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid email or password";
                return View();
            }
          
        }

        //admin Login End

        //admin Logout Start
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("mysession");
            return RedirectToAction("Login");
        }
        //admin Logout End

        //admin Edit Start
        public IActionResult admin_Edit()
        {
            // Session se admin_id ko string ke roop mein get kar rahe hain
            var session = HttpContext.Session.GetString("mysession");

            // Session se value lene ke baad, usse integer mein convert karna hoga
            if (int.TryParse(session, out int adminId))
            {
                // Ab admin_id ko integer ke roop mein find method mein pass karenge
                var data = context.admins.Find(adminId);
                return View(data);
            }
            else
            {
                // Agar session mein invalid ya null value aayi toh ek error handle kar sakte hain
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult admin_Edit(admin ad, IFormFile admin_image)
        {
            var data = context.admins.Where(a => a.admin_id == ad.admin_id).FirstOrDefault();

            if (data != null)
            {
                if (admin_image != null && admin_image.Length > 0)
                {
                    var old_img = Path.Combine(Env.WebRootPath, "Images", data.admin_image);
                    if (System.IO.File.Exists(old_img))
                    {
                        System.IO.File.Delete(old_img);
                    }
                    var location = Path.Combine(Env.WebRootPath, "Images", admin_image.FileName);
                    FileStream file = new FileStream(location, FileMode.Create);
                    admin_image.CopyTo(file);
                    data.admin_image = admin_image.FileName;
                }
                else
                {
                    ad.admin_image = data.admin_image;
                }

                // Update other fields
                data.admin_name = ad.admin_name;
                data.admin_email = ad.admin_email;
                data.admin_password = ad.admin_password;

                // Save changes to the database
                context.SaveChanges();

                // Set TempData to show success message
                TempData["SuccessMessage"] = "Data updated successfully!";

                // Redirect to the "Student_table" action
                return RedirectToAction("Index");
            }

            // If admin is not found, you can handle it differently
            TempData["ErrorMessage"] = "Admin not found!";
            return RedirectToAction("admin_Edit");
        }

        //admin Edit End


        //State Create Start

        public IActionResult State_Create()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                return View("State_Create");
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }

        [HttpPost]
       public IActionResult State_Create(State state)
        {
            if (ModelState.IsValid)
            {
                context.states.Add(state);
                context.SaveChanges();
                TempData["SuccessMessage"] = "State added successfully!";
                return RedirectToAction("State_table");
            }
            else
            {
                TempData["error"] = "State added failed!";

                return View(state);

            }
        }

        public IActionResult State_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var data = context.states.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }

        public IActionResult state_Edit(int id)
        {
            var data = context.states.Find(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult state_Edit(State state)
        {
            if (ModelState.IsValid)
            {
                context.states.Update(state);
                context.SaveChanges();
                TempData["SuccessMessage"] = "State updated successfully!";
                return RedirectToAction("State_table");
            }
            else
            {
                TempData["error"] = "State update failed!";
                return View(state);
            }
        }
        [HttpGet]
        public IActionResult State_delete(int id)
        {
            var data = context.states.Find(id);
            if (data != null)
            {
                context.states.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "State deleted successfully!";
            }
            else
            {
                TempData["error"] = "State not found!";
            }
            return RedirectToAction("State_table");
        }

        //State Create End

        //Cutomer Table

     
        public IActionResult Custamer_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var customers = context.customers.Include(c => c.state).ToList();
                return View(customers);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public IActionResult cutomer_edit(int id)
        {
            var data = context.customers.Find(id);
            return View(data);
        }
        [HttpPost]
        public IActionResult cutomer_edit(customer customer)
        {
            if (ModelState.IsValid)
            {
                context.customers.Update(customer);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("Custamer_table");
            }
            else
            {
                TempData["error"] = "Customer update failed!";
                return View(customer);
            }
        }

        [HttpGet]
        public IActionResult cutomer_delete(int id)
        {
            var data = context.customers.Find(id);
            if (data != null)
            {
                context.customers.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Customer deleted successfully!";
            }
            else
            {
                TempData["error_delete"] = "Customer not found!";
            }
            return RedirectToAction("Custamer_table");
        }

            //catogery 
        public IActionResult Catogery()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                return View("Catogery");
            }
            else
            {
                return RedirectToAction("Login");
            }

          
        }

        [HttpPost]
        public IActionResult Catogery(catogery cat)
        {
            if (ModelState.IsValid)
            {
                context.catogerys.Add(cat);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Catogery added successfully!";
                return RedirectToAction("catogery_table");
            }
            else
            {
                TempData["error"] = "Catogery added failed!";
                return View();
            }
        }

        public IActionResult catogery_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var data = context.catogerys.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
         
        }

        public IActionResult catogery_edit(int id)
        {
            var data = context.catogerys.Find(id);
            return View(data);
        }
        [HttpPost]
        public IActionResult catogery_edit(catogery cat)
        {
            if (ModelState.IsValid)
            {
                context.catogerys.Update(cat);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Catogery updated successfully!";
                return RedirectToAction("catogery_table");
            }
            else
            {
                TempData["error"] = "Catogery update failed!";
                return View(cat);
            }
        }


        [HttpGet]
        public IActionResult catogery_delete(int id)
        {
            var data = context.catogerys.Find(id);
            if (data != null)
            {
                context.catogerys.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Catogery deleted successfully!";
            }
            else
            {
                TempData["error_delete"] = "Catogery not found!";
            }

            // 👇 This will bring you back to the category table page after delete
            return RedirectToAction("catogery_table");
        }

        //catogery End

        //product Start

        public IActionResult product()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin == null)
            {
                return RedirectToAction("Login");
            }
         
            
            // Ensure context is properly initialized and states are available
            if (context == null || !context.catogerys.Any())
            {
                TempData["error_cus"] = "No catogerys available.";
                return RedirectToAction("Index");  // Or another appropriate action
            }

            // Create SelectList for state dropdown
            ViewBag.cat_id = new SelectList(context.catogerys, "cat_id", "cat_name");

            return View();
        }
        [HttpPost]
        public IActionResult product(product pro, IFormFile product_image)
        {
            if (product_image != null && product_image.Length > 0)
                {
                    var location = Path.Combine(Env.WebRootPath, "Images", product_image.FileName);
                    FileStream file = new FileStream(location, FileMode.Create);
                    product_image.CopyTo(file);
                    pro.product_image = product_image.FileName;
                }
                context.products.Add(pro);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("product_table");
            }
         
        

        public IActionResult product_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var data = context.products.Include(p => p.Catogery).ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }

        public IActionResult product_edit(int id)
        {
            var data = context.products.Find(id);
            ViewBag.cat_id = new SelectList(context.catogerys, "cat_id", "cat_name");
            return View(data);
        }
        [HttpPost]
        public IActionResult product_edit(IFormFile product_image,int id,product pro)
        {
            var data = context.products.Find(id);
            if (data != null)
            {
                if (product_image != null && product_image.Length > 0)
                {
                    var old_img = Path.Combine(Env.WebRootPath, "Images", data.product_image);
                    if (System.IO.File.Exists(old_img))
                    {
                        System.IO.File.Delete(old_img);
                    }
                    var location = Path.Combine(Env.WebRootPath, "Images", product_image.FileName);
                    FileStream file = new FileStream(location, FileMode.Create);
                    product_image.CopyTo(file);
                    data.product_image = product_image.FileName;
                }
                else
                {
                    pro.product_image = data.product_image;
                }
                // Update other fields
                data.product_name = pro.product_name;
                data.product_price = pro.product_price;
                data.product_description = pro.product_description;
                data.product_quantity = pro.product_quantity;
                data.cat_id = pro.cat_id;
                // Save changes to the database
                context.SaveChanges();
                // Set TempData to show success message
                TempData["SuccessMessage"] = "Product updated successfully!";
                // Redirect to the "Student_table" action
                return RedirectToAction("product_table");
            }
            // If admin is not found, you can handle it differently
            TempData["ErrorMessage"] = "Product not found!";
            return RedirectToAction("product_edit");
        }
        [HttpGet]
        public IActionResult product_delete(int id)
        {
            var data = context.products.Find(id);
            if (data != null)
            {
                context.products.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            else
            {
                TempData["error_delete"] = "Product not found!";
            }
            return RedirectToAction("product_table");
        }


        public IActionResult feedback_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var data = context.feedbacks.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
           
        }

        [HttpGet]
        public IActionResult feedback_delete(int id)
        {
            var data = context.feedback.Find(id);
            if (data != null)
            {
                context.feedback.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Feedback deleted successfully!";
            }
            else
            {
                TempData["error"] = "Feedback not found!";
            }
        
            // 👇 This will bring you back to the category table page after delete
            return RedirectToAction("feedback_table");
        }
        public IActionResult cart_table()
        {
            string admin = HttpContext.Session.GetString("mysession");
            if (admin != null)
            {
                var data = context.carts
                    // Ensure product is loaded
                    .Include(c => c.customers)
                     .Include(p => p.products)
                      .ToList();

                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
          
        }




        public IActionResult cart_update(int id)
        {
            var data = context.carts.Find(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult cart_update(cart cart)
        {
                context.carts.Update(cart);
                context.SaveChanges();
                return RedirectToAction("cart_table");
        
        }

        [HttpGet]
        public IActionResult cart_delete(int id)
        {
            var data = context.carts.Find(id);
            if (data != null)
            {
                context.carts.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Cart deleted successfully!";
            }
            else
            {
                TempData["error"] = "Cart not found!";
            }
        
            // 👇 This will bring you back to the category table page after delete
            return RedirectToAction("cart_table");
        }

        public IActionResult order_table()
        {
            var data = context.order_detail_view.ToList();
            return View(data);
        }

        public IActionResult orderedit(int id)
        {
            var data = context.order_detail_view.FirstOrDefault(o => o.order_id == id);
            if (data == null)
                return NotFound();

            return View(data);
        }

        // POST: Save the updated status
        [HttpPost]
        public IActionResult OrderEdit(int order_id, string order_status)
        {
            // Use order_items table
            var orderItem = context.order_items.FirstOrDefault(oi => oi.order_id == order_id);
            if (orderItem == null)
                return NotFound();

            orderItem.order_status = order_status;
            context.SaveChanges();

            return RedirectToAction("order_table");
        }
        [HttpGet]
        public IActionResult DeleteOrder(int order_id)
        {
            var items = context.order_items.Where(oi => oi.order_id == order_id).ToList();

            if (items == null || items.Count == 0)
            {
                return NotFound();
            }

            context.order_items.RemoveRange(items);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Cart deleted successfully!";
            return RedirectToAction("order_table");
        }




    }
}
