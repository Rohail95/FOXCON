using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountsController : BaseAdminController
    {
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;

        public DiscountsController(IDiscountService discountService, IProductService productService)
        {
            _discountService = discountService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return View(discounts);
        }

        public async Task<IActionResult> Create()
        {
            var allProducts = await _productService.GetAllProductsAsync();
            var viewModel = new DiscountViewModel
            {
                Discount = new Discount { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(1), IsActive = true },
                Products = new MultiSelectList(allProducts.OrderBy(p => p.Name), "ProductId", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedProductIds.Any())
                {
                    var allProducts = await _productService.GetAllProductsAsync();
                    viewModel.Discount.Products = allProducts
                        .Where(p => viewModel.SelectedProductIds.Contains(p.ProductId)).ToList();
                }
                await _discountService.CreateDiscountAsync(viewModel.Discount);
                return RedirectToAction(nameof(Index));
            }

            var productsForDropdown = await _productService.GetAllProductsAsync();
            viewModel.Products = new MultiSelectList(productsForDropdown.OrderBy(p => p.Name), "ProductId", "Name", viewModel.SelectedProductIds);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var discount = await _discountService.GetDiscountByIdAsync(id.Value);
            if (discount == null) return NotFound();

            var allProducts = await _productService.GetAllProductsAsync();
            var viewModel = new DiscountViewModel
            {
                Discount = discount,
                SelectedProductIds = discount.Products.Select(p => p.ProductId).ToList(),
                Products = new MultiSelectList(allProducts.OrderBy(p => p.Name), "ProductId", "Name", discount.Products.Select(p => p.ProductId))
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DiscountViewModel viewModel)
        {
            if (id != viewModel.Discount.DiscountId) return NotFound();

            if (ModelState.IsValid)
            {
                await _discountService.UpdateAsync(viewModel.Discount, viewModel.SelectedProductIds);
                return RedirectToAction(nameof(Index));
            }

            var allProducts = await _productService.GetAllProductsAsync();
            viewModel.Products = new MultiSelectList(allProducts.OrderBy(p => p.Name), "ProductId", "Name", viewModel.SelectedProductIds);
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var discount = await _discountService.GetDiscountByIdAsync(id.Value);
            if (discount == null) return NotFound();
            return View(discount);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _discountService.DeleteDiscountAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}