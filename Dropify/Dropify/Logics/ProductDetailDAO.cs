﻿using Dropify.Models;

namespace Dropify.Logics
{
    public class ProductDetailDAO
    {
        // Lấy tất cả product detail từ database
        // Người viết: Bùi Quang Minh
        // Ngày: 16/2/2024
        public List<ProductDetail> GetAllProductDetails()
        {
            using (var db = new prn211_dropshippingContext())
            {
                return db.ProductDetails.ToList();
            }
        }
        // Lấy product detail theo id
        // Người viết: Bùi Quang Minh
        // Ngày: 16/2/2024
        public List<ProductDetail> GetProductDetailById(int id)
        {
            using (var db = new prn211_dropshippingContext())
            {
                return db.ProductDetails.Where(x => x.ProductId == id).ToList();
            }
        }
        //Thêm product detail vào database
        // Người viết: Bùi Quang Minh
        // Ngày: 16/2/2024
        public void AddProductDetail(ProductDetail p)
        {
            using (var db = new prn211_dropshippingContext())
            {
                db.ProductDetails.Add(p);
                db.SaveChanges();
            }
        }

        //Update product detail vào database
        // Người viết: NQTuan
        // Ngày: 04/03/2024
        public void UpdateProductDetail(ProductDetail p)
        {
            using (var db = new prn211_dropshippingContext())
            {
                db.ProductDetails.Update(p);
                db.SaveChanges();
            }
        }
        //Update product detail vào database
        // Người viết: NQTuan
        // Ngày: 04/03/2024
        public ProductDetail getProductDetailByPdId(int id)
        {
            using (var db = new prn211_dropshippingContext())
            {
                ProductDetail productD =  db.ProductDetails.FirstOrDefault(pd => pd.ProductDetailId == id);
                return productD;
               
            }
        }
    }
}
