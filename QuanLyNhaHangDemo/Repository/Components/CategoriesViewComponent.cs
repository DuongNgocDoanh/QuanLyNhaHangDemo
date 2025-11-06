using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHangDemo.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public CategoriesViewComponent(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Chỉ lấy những category đang active (Status = 1)
            var activeCategories = await _dataContext.Categories
                .Where(c => c.Status == 1)
                .ToListAsync();

            return View(activeCategories);
        }
    }
}
