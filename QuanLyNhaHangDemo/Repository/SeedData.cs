using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangDemo.Models;

namespace QuanLyNhaHangDemo.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();

            // Nếu chưa có dữ liệu thì mới thêm
            if (!_context.Products.Any())
            {
                var Bia = new CategoryModel
                {
                    Name = "Bia",
                    Slug = "bia",
                    Description = "Bia is good",
                    Status = 1
                };

                var NuocNgot = new CategoryModel
                {
                    Name = "Coca",
                    Slug = "coca",
                    Description = "Coca is good",
                    Status = 1
                };

                var Cocacola = new BrandModel
                {
                    Name = "Coca",
                    Slug = "coca",
                    Description = "Coca is good",
                    Status = 1
                };

                // Thêm category và brand trước
                _context.Categories.AddRange(Bia, NuocNgot);
                _context.Brands.Add(Cocacola);
                _context.SaveChanges();

                // ✅ AddRange đúng cú pháp
                _context.Products.AddRange(
                    new ProductModel
                    {
                        Name = "CocaNho",
                        Slug = "cocanho",
                        Description = "hihi",
                        Image = "1.jpg",
                        Category = NuocNgot,
                        Brand = Cocacola,
                        Price = 10
                    },
                    new ProductModel
                    {
                        Name = "CocaTo",
                        Slug = "cocato",
                        Description = "hihi",
                        Image = "1.jpg",
                        Category = NuocNgot,
                        Brand = Cocacola,
                        Price = 15
                    }
                );

                _context.SaveChanges();
            }
        }
    }
}
