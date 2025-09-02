using BusinessLayer.Services;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService; 

        public ProductsController(IProductService productService, IDiscountService discountService) 
        {
            _productService = productService;
            _discountService = discountService; 
        }


        public async Task<IActionResult> Index(string? category, string? searchString)
        {
            var products = await _productService.GetAllProductsWithDiscountsAsync();
            var siteWideDiscounts = await _discountService.GetActiveSiteWideDiscountsAsync();
            var firstSiteWideDiscount = siteWideDiscounts.FirstOrDefault();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category?.Name != null && p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                ViewData["SearchTerm"] = searchString;
            }

            var productViewModels = new List<ProductViewModel>();
            foreach (var product in products)
            {
                var viewModel = new ProductViewModel { Product = product };
                var activeDiscount = product.Discounts?.FirstOrDefault(d => d.IsActive == true && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow) ?? firstSiteWideDiscount;

                if (activeDiscount != null)
                {
                    viewModel.IsOnSale = true;
                    viewModel.OriginalPrice = product.Price;
                    if (activeDiscount.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                    {
                        viewModel.DisplayPrice = product.Price * (1 - (activeDiscount.DiscountValue / 100));
                    }
                    else
                    {
                        viewModel.DisplayPrice = Math.Max(0, product.Price - activeDiscount.DiscountValue);
                    }
                }
                else
                {
                    viewModel.IsOnSale = false;
                    viewModel.DisplayPrice = product.Price;
                    viewModel.OriginalPrice = null;
                }
                productViewModels.Add(viewModel);
            }

            ViewBag.FeaturedProducts = productViewModels
                .Where(p => p.IsOnSale)
                .OrderByDescending(p => (p.OriginalPrice - p.DisplayPrice)) 
                .Take(4)
                .ToList();
           

            return View(productViewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            var siteWideDiscounts = await _discountService.GetActiveSiteWideDiscountsAsync();
            var firstSiteWideDiscount = siteWideDiscounts.FirstOrDefault();

            var viewModel = new ProductViewModel { Product = product };

            var activeDiscount = product.Discounts?
                .FirstOrDefault(d => d.IsActive == true && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow);

            if (activeDiscount == null)
            {
                activeDiscount = firstSiteWideDiscount;
            }

            if (activeDiscount != null)
            {
                viewModel.IsOnSale = true;
                viewModel.OriginalPrice = product.Price;
                if (activeDiscount.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                {
                    viewModel.DisplayPrice = product.Price * (1 - (activeDiscount.DiscountValue / 100));
                }
                else
                {
                    viewModel.DisplayPrice = Math.Max(0, product.Price - activeDiscount.DiscountValue);
                }
            }
            else
            {
                viewModel.IsOnSale = false;
                viewModel.DisplayPrice = product.Price;
                viewModel.OriginalPrice = null;
            }
            return View(viewModel);
        }
    }
}