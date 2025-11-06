using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHangDemo.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public BrandsViewComponent(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Chỉ lấy những brand có Status = 1 (hiển thị)
            var activeBrands = await _dataContext.Brands
                .Where(b => b.Status == 1)
                .ToListAsync();

            return View(activeBrands);
        }
    }
}
