using BusinessLayer.Services;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItemById(int id)
        {
            var cartItem = await _cartItemService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<ActionResult<CartItem>> CreateCartItem([FromBody] CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("CartItem object is null");
            }

            try
            {
                await _cartItemService.CreateCartItemAsync(cartItem);
                return CreatedAtAction(nameof(GetCartItemById), new { id = cartItem.CartItemId }, cartItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return BadRequest("ID mismatch");
            }
            await _cartItemService.UpdateCartItemAsync(cartItem);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            await _cartItemService.DeleteCartItemAsync(id);
            return NoContent();
        }
    }
}