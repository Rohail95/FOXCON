using DatabaseLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EcommerceWeb.ViewModels
{
    public class DiscountViewModel
    {
        public Discount Discount { get; set; } = new();

        public List<int> SelectedProductIds { get; set; } = new();

        public MultiSelectList? Products { get; set; }
    }
}