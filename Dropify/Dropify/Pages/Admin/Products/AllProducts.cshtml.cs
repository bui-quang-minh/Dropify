﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dropify.Models;
using Dropify.Logics;
using Newtonsoft.Json;

namespace Dropify.Pages.Admin.ManageProduct
{
    public class ProductModel : BasePageModel
    {
        [BindProperty]
        public ProductDetail productDetail { get; set; }

        [BindProperty]
        public Models.Product product { get; set; }
        public Dropify.Models.prn211_dropshippingContext con;
        public ProductDAO pd = new ProductDAO();
        public ProductModel(Dropify.Models.prn211_dropshippingContext context)
        {
            con = context;
        }
       
        public IList<Category> categories { get; set; }
        public IList<Models.Supplier> suppliers { get; set; }
        public IList<Models.Product> ListProduct { get;set; }
        public IList<Models.Product> ListProduct1 { get; set; }
        public IList<Models.Product> ListProduct2 { get; set; }
        public IList<Models.Product> ListProduct3 { get; set; }
        public IList<Models.Product> ListProduct4 { get; set; }

        public List<Models.Product> lp = new List<Models.Product>();
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
                    ListProduct = pd.GetAllProducts(); // all sản phẩm 
                   
                    categories = con.Categories.ToList();
                    suppliers = con.Suppliers.ToList();
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
        public IActionResult OnPostDelete()
        {
            try
            {
                int pid = int.Parse(Request.Form["p_id"].ToString());

                var pro = con.Products.Find(pid);
                if (pro != null)
                {
                    if (!pro.Status.Equals("Cancel"))
                    {
                        pro.Status = "Cancel";
                        con.Products.Update(pro);
                        con.SaveChanges();

                    }
                }
                return RedirectToPage("AllProducts");

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
        public IActionResult OnPostRestore()
        {

            int pid = int.Parse(Request.Form["restore_id"].ToString());

            var pro = con.Products.Find(pid);
            if (pro != null)
            { 
                    pro.Status = "Release";
                    con.Products.Update(pro);
                    con.SaveChanges();
            }
            return RedirectToPage("AllProducts");
        }

      
       public IActionResult OnPostEdit()
        {
            var p = con.Products.Find(product.ProductId);
            if (p == null)
            {
                 return NotFound();
            }
            
            else
            {
                p.Name = product.Name;
                p.CategoryId = product.CategoryId;
                p.SupplierId = product.SupplierId;
                p.OriginalPrice = product.OriginalPrice;
                p.SellOutPrice = product.SellOutPrice;
                p.CreatedDate = product.CreatedDate;
                p.StartDate = product.StartDate;
                p.EndDate = product.EndDate;
                p.Shipdate = product.Shipdate;
                p.Status = product.Status;
               
                con.Products.Update(p);
                con.SaveChanges();
                return RedirectToPage("AllProducts");
            }

           

        }


    }
}
