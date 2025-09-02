using BusinessLayer.Services;
using DatabaseLayer.Models;
using EcommerceWeb.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EcommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IWebHostEnvironment webHostEnvironment) : BaseAdminController
    {
        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await productService.GetAllProductsAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropDownList();
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = await SaveImageAsync(viewModel.ImageFile);

                Product newProduct = new()
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    CategoryId = viewModel.CategoryId,
                    Sku = viewModel.Sku,
                    ImageUrl = uniqueFileName
                };

                await productService.CreateProductAsync(newProduct);
                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesDropDownList(viewModel.CategoryId);
            return View(viewModel);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            var viewModel = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                Sku = product.Sku,
                ExistingImageUrl = product.ImageUrl
            };

            await PopulateCategoriesDropDownList(product.CategoryId);
            return View(viewModel);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                var productToUpdate = await productService.GetProductByIdAsync(id);
                if (productToUpdate == null) return NotFound();

                if (viewModel.ImageFile != null)
                {
                    productToUpdate.ImageUrl = await SaveImageAsync(viewModel.ImageFile);
                }

                productToUpdate.Name = viewModel.Name;
                productToUpdate.Description = viewModel.Description;
                productToUpdate.Price = viewModel.Price;
                productToUpdate.StockQuantity = viewModel.StockQuantity;
                productToUpdate.CategoryId = viewModel.CategoryId;
                productToUpdate.Sku = viewModel.Sku;

                await productService.UpdateProductAsync(productToUpdate);
                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesDropDownList(viewModel.CategoryId);
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateCategoriesDropDownList(object? selectedCategory = null)
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "Name", selectedCategory);
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null) return null;

            string wwwRootPath = webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwRootPath, "images", "products");
            Directory.CreateDirectory(uploadPath);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadPath, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(fileStream);
            return Path.Combine("/images/products/", uniqueFileName).Replace("\\", "/");
        }
    }
}