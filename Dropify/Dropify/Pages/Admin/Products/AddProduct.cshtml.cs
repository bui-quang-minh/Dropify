using CloudinaryDotNet.Actions;
using Dropify.Logics;
using Dropify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Dropify.Pages.Admin.Products
{
    public class AddProductModel : BasePageModel
    {
        public List<Models.Category> availableCategory = new List<Models.Category>();
        public User user;
        public UserDetail userDetail;
        public IActionResult OnGet()
        {
            string userString = HttpContext.Session.GetString("user");

            if (userString != null)
            {
                user = JsonConvert.DeserializeObject<User>(userString);
                UserDetailDAO userDAO = new UserDetailDAO();
                userDetail = userDAO.GetUserDetailById(user.Uid);
                if (userDetail.Admin == true)
                {
                    availableCategory = new CategoryDAO().GetAvailableCategories();
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                return RedirectToPage("/Login");
            }
        }
       
        public IActionResult OnPostAdd(List<IFormFile> images)
        {
            var name = Request.Form["productName"];
            var category = Request.Form["category"];
            var supplier = Request.Form["supplier"];
            var p_color = Request.Form["color"];
            var p_size = Request.Form["size"];
            var p_options = Request.Form["options"];
            var originalPrice = Request.Form["originalPrice"];
            var sellOutPrice = Request.Form["sellOutPrice"];
            var description = Request.Form["description"];
            //Create new product
            Models.Product p = new Models.Product();
            p.Name = name;
            p.SupplierId = int.Parse(supplier);
            p.CategoryId = int.Parse(category);
            p.OriginalPrice = decimal.Parse(originalPrice);
            p.SellOutPrice = decimal.Parse(sellOutPrice);
            p.CreatedDate = DateTime.Now;
            p.StartDate = DateTime.Now.AddDays(5);
            p.EndDate = DateTime.Now.AddDays(35);
            p.Shipdate = DateTime.Now.AddDays(50);
            //Product Status:
            //  - Release: Product is on only for advertising purpose, not for sale (between Created Date and Started Date)
            //  - Selling: Product is on sale (between Started Date and End Date)
            //  - Shipping: Product is on shipping (between End Date and Ship Date)
            //  - Success: Product is done selling (after Ship Date)
            //  - Cancel: Product is canceled (Cancel by Administrator)
            p.Status = "Release";
            p.CreatedBy = 1;
            p.ModifiedBy = 1;

            int productId = new ProductDAO().AddProduct(p);

            //Save product details
            //P_COLOR
            if (!string.IsNullOrEmpty(p_color))
            {
                string[] colors = p_color.ToString().Split(';');
                foreach (var item in colors)
                {
                    Models.ProductDetail p_c = new Models.ProductDetail();
                    p_c.Type = "P_COLOR";
                    p_c.ProductId = productId;
                    p_c.Attribute = item;
                    new ProductDetailDAO().AddProductDetail(p_c);
                }
            }
            //P_SIZE
            if (!string.IsNullOrEmpty(p_size))
            {
                string[] sizes = p_size.ToString().Split(';');
                foreach (var item in sizes)
                {
                    Models.ProductDetail p_s = new Models.ProductDetail();
                    p_s.Type = "P_SIZE";
                    p_s.Attribute = item;
                    p_s.ProductId = productId;
                    new ProductDetailDAO().AddProductDetail(p_s);
                }
            }
            //P_OPTIONS
            if (!string.IsNullOrEmpty(p_options))
            {
                string[] options = p_options.ToString().Split(';');
                foreach (var item in options)
                {
                    Models.ProductDetail p_o = new Models.ProductDetail();
                    p_o.Type = "P_OPTIONS";
                    p_o.Attribute = item;
                    p_o.ProductId = productId;
                    new ProductDetailDAO().AddProductDetail(p_o);
                }
            }
            //P_IMAGE_THUMBNAIL, P_IMAGE_CONTENTS
            if (images != null && images.Count > 0)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    Models.ProductDetail p_i = new Models.ProductDetail();

                    if (i == 0)
                    {
                        p_i.Type = "P_IMAGE_THUMBNAIL";
                    }
                    else
                    {
                        p_i.Type = "P_IMAGE_CONTENTS";
                    }

                    p_i.ProductId = productId;
                    CloudinarySettings cs = new CloudinarySettings();
                    ImageUploadResult res = cs.CloudinaryUpload(images[i]);
                    p_i.Attribute = res.SecureUrl.ToString();
                    new ProductDetailDAO().AddProductDetail(p_i);
                }
            }
            //P_DESCRIPTION

            if (!string.IsNullOrEmpty(description)) { 
                Models.ProductDetail p_d = new Models.ProductDetail();
                p_d.Type = "P_DESCRIPTION";
                p_d.ProductId = productId;
                p_d.Attribute = description;
                new ProductDetailDAO().AddProductDetail(p_d);
            }
            return RedirectToPage("/Admin/Products/AllProducts");
        }
    }
}
