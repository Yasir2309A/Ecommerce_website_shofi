using System.Diagnostics;
using System.Text;
using Ecommerce_website.Email;
using Ecommerce_website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce_website.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext context;
        private readonly EmailHelper emailHelper;

        public HomeController(MyDbContext context, EmailHelper  emailHelper)
        {
            this.context = context;
            this.emailHelper = emailHelper;
        }

        public IActionResult Index()
        {
            List<product> product = context.products.ToList();
            ViewData["product"] = product;

            ViewBag.check = HttpContext.Session.GetString("user");

            return View();
        }

        public IActionResult _show_Product()
        {
            ViewBag.check = HttpContext.Session.GetString("user");
            var products = context.products.ToList();
            return PartialView(products);
        }

        public IActionResult product_detail(int id)
        {
          

            ViewBag.check = HttpContext.Session.GetString("user");

            var product = context.products.FirstOrDefault(p => p.product_id == id);

            // If no product is found, return a 404 Not Found response
            if (product == null)
            {
                return NotFound();  // You can return a custom error page instead of this if needed.
            }

            // Return the view with the single product object
            return View(product);
        }


        //Cutomer Table

        public IActionResult Customer_Create()
        {
            List<product> product = context.products.ToList();
            ViewData["product"] = product;

            ViewBag.check = HttpContext.Session.GetString("user");

            // Ensure context is properly initialized and states are available
            if (context == null || !context.states.Any())
            {
                TempData["error_cus"] = "No states available.";
                return RedirectToAction("Index");  // Or another appropriate action
            }

            // Create SelectList for state dropdown
            ViewBag.state_id = new SelectList(context.states, "state_id", "state_name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Customer_Create(customer model)
        {
            // Hash the password before storing or sending to session
            model.coustomer_password = HashPassword(model.coustomer_password);

            // Generate verification code
            Random rnd = new Random();
            model.VerificationCode = rnd.Next(100000, 999999).ToString();

            // Store in session
            HttpContext.Session.SetString("VerificationCode", model.VerificationCode);
            HttpContext.Session.SetString("TempCustomerEmail", model.customer_email);
            HttpContext.Session.SetString("TempCustomerData", JsonConvert.SerializeObject(model));

            // Send email
            string subject = "Email Verification Code";
            string body = $"Your verification code is <b>{model.VerificationCode}</b>";
            await emailHelper.SendEmailAsync(model.customer_email, subject, body);

            return RedirectToAction("VerifyEmail");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyEmail(string code)
        {
            string sessionCode = HttpContext.Session.GetString("VerificationCode");
            if (code == sessionCode)
            {
                string json = HttpContext.Session.GetString("TempCustomerData");
                var customer = JsonConvert.DeserializeObject<customer>(json);

                context.customers.Add(customer);
                context.SaveChanges();

                TempData["success"] = "Registration successful!";
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("TempCustomerData");

                return RedirectToAction("user_login");
            }

            TempData["error"] = "Invalid verification code.";
            return View();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public IActionResult user_login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult user_login(customer cust)
        {
            // Hash the entered password
            string hashedPassword = HashPassword(cust.coustomer_password);

            // Compare with stored hashed password
            var user = context.customers
                .FirstOrDefault(x => x.customer_email == cust.customer_email && x.coustomer_password == hashedPassword);

            if (user != null)
            {
                HttpContext.Session.SetString("user_id", user.customer_id.ToString());
                HttpContext.Session.SetString("user", user.customer_name);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid email or password";
                return View();
            }
        }


        public IActionResult user_logout()
        {
            HttpContext.Session.Clear(); // Clear session

            // Prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index");
        }

        public IActionResult Custamer_table()
        {
            //string admin = HttpContext.Session.GetString("mysession");
            //if (admin != null)
            //{
            //    return View("Index");
            //}
            //else
            //{
            //    return RedirectToAction("Login");
            //}

            var customers = context.customers.Include(c => c.state).ToList();

            return View(customers);

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





        public IActionResult user_table()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user_id")))
            {
                return RedirectToAction("user_login");
            }

            var session = HttpContext.Session.GetString("user_id");
            //var user = context.customers.FirstOrDefault(x => x.customer_id == int.Parse(session));
            var user = context.customers
                .Include(x => x.state)
                .FirstOrDefault(x => x.customer_id == int.Parse(session));

            if (user == null)
            {
                return RedirectToAction("user_login");
            }
            List<product> product = context.products.ToList();
            ViewData["product"] = product;
            return View(user);
        }


        public IActionResult profile_update()
        {
            // Session se admin_id ko string ke roop mein get kar rahe hain
            var session = HttpContext.Session.GetString("user_id");
            if (session != null)
            {
                // Session se admin_id ko int mein convert kar rahe hain
                var user = context.customers
                    .Include(x => x.state)
                    .FirstOrDefault(x => x.customer_id == int.Parse(session));
                ViewBag.state_id = new SelectList(context.states, "state_id", "state_name");
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    TempData["error"] = "User not found!";
                    return RedirectToAction("user_login");

                }

            }
            List<product> product = context.products.ToList();
            ViewData["product"] = product;
            return RedirectToAction("user_login");

        }
        [HttpPost]
        public IActionResult profile_update(customer cut)
        {
            var data = context.customers.FirstOrDefault(a => a.customer_id == cut.customer_id);
            if (data != null)
            {
                data.customer_name = cut.customer_name;
                data.customer_email = cut.customer_email;

                // ? Only hash and update password if user entered a new one
                if (!string.IsNullOrWhiteSpace(cut.coustomer_password))
                {
                    data.coustomer_password = HashPassword(cut.coustomer_password);
                }

                data.customer_phone = cut.customer_phone;
                data.Country = cut.Country;
                data.state_id = cut.state_id;
                data.address = cut.address;

                context.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            else
            {
                TempData["error"] = "Profile update failed!";
            }

            return RedirectToAction("user_table");
        }




        public IActionResult Feedback()
        {
            List<product> product = context.products.ToList();
            ViewData["product"] = product;

            ViewBag.check = HttpContext.Session.GetString("user");
            return View();
        }

        [HttpPost]
        public IActionResult Feedback(feedback feedback)
        {
            if (ModelState.IsValid)
            {
               var data =   context.feedbacks.Add(feedback);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Feedback submitted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Feedback submission failed!";
                return View(feedback);
            }
        }

        public IActionResult Shop()
        {
            List<product> product = context.products.ToList();
            ViewData["product"] = product;

            ViewBag.check = HttpContext.Session.GetString("user");
            return View();
        }

       
        public IActionResult AddToCart(int pro_id, int quantity)
        {
            var session_login = HttpContext.Session.GetString("user_id");
            if (session_login == null)
            {
                return RedirectToAction("user_login");
            }
            else
            {
                var product = context.products.FirstOrDefault(p => p.product_id == pro_id);
                if (product == null)
                {
                    return NotFound();
                }

                var cart = new cart
                {
                    customer_id = int.Parse(session_login),
                    product_id = pro_id,
                    status = "Pending",
                    quantity = quantity,
                    total_price = product.product_price * quantity
                };

                context.carts.Add(cart);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Product added to cart successfully!";
                return RedirectToAction("Shop");
            }
        }

        public IActionResult cart_show()
        {
            List<product> product = context.products.ToList();
            ViewData["product"] = product;

            ViewBag.check = HttpContext.Session.GetString("user");

            var session_login = HttpContext.Session.GetString("user_id");
            if (session_login == null)
            {
                return RedirectToAction("user_login");
            }
            else
            {
                var cartItems = context.carts.Include(c => c.products).Where(c => c.customer_id == int.Parse(session_login)).ToList();
                return View(cartItems);
            }
        }

        [HttpGet]
        public IActionResult cart_deleted(int id)
        {
            var data = context.carts.Find(id);
            if (data != null)
            {
                context.carts.Remove(data);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Product removed from cart successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Product not found in cart!";
            }
            return RedirectToAction("cart_show");
        }

        [HttpPost]
        public IActionResult buynow(int pro_id, int quantity)
        {
            var session_login = HttpContext.Session.GetString("user_id");
            if (session_login == null)
            {
                return RedirectToAction("user_login");
            }

            var product = context.products.FirstOrDefault(p => p.product_id == pro_id);
            if (product == null)
            {
                return NotFound();
            }

            // Save product info to TempData (or Session)
            TempData["pro_id"] = pro_id;
            TempData["quantity"] = quantity;

            return RedirectToAction("checkout");
        }

        [HttpPost]
        public IActionResult ProductAction(int pro_id, int quantity, string submitType)
        {
            if (submitType == "add")
            {
                return AddToCart(pro_id, quantity); // ?? yahi tera pura logic use ho raha hai
            }
            else if (submitType == "buy")
            {
                return buynow(pro_id, quantity);
            }

            return RedirectToAction("Shop");
        }

        public IActionResult checkout()
        {
           

            if (TempData["pro_id"] == null || TempData["quantity"] == null)
            {
                return RedirectToAction("Shop");
            }

            // ?? Get session login user ID
            var session_login = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(session_login))
            {
                TempData["error"] = "Please login first.";
                return RedirectToAction("user_login");
            }

            int pro_id = Convert.ToInt32(TempData["pro_id"]);
            int quantity = Convert.ToInt32(TempData["quantity"]);
            int user_id = int.Parse(session_login);

            var product = context.products
                                 .Include(p => p.Catogery)
                                 .FirstOrDefault(p => p.product_id == pro_id);

            var customer = context.customers
                                  .Include(c => c.state)
                                  .FirstOrDefault(c => c.customer_id == user_id);

            ViewBag.Product = product;
            ViewBag.Quantity = quantity;
            ViewBag.Customer = customer;
            ViewBag.States = context.states.ToList();

            ViewBag.check = HttpContext.Session.GetString("user");

            return View();
        }


        [HttpPost]
        public IActionResult checkout(order orderData, int pro_id, int quantity)
        {
            var session_login = HttpContext.Session.GetString("user_id");
            if (session_login == null)
            {
                return RedirectToAction("user_login");
            }

            // 1. Create order
            orderData.customer_id = int.Parse(session_login);
            context.orders.Add(orderData);
            context.SaveChanges(); // order_id is now generated

            // 2. Create order_item
            var product = context.products.FirstOrDefault(p => p.product_id == pro_id);
            var orderItem = new order_item
            {
                order_id = orderData.order_id,
                product_id = pro_id,
                quantity = quantity,
                order_status = "Pending",
                total = product.product_price * quantity
            };

            context.order_items.Add(orderItem);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Order placed successfully! Our team confirmation call you thanks";
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
