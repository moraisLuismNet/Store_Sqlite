﻿using Store.DTOs;
using Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Services;
using Microsoft.AspNetCore.Authorization;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly StoreContext _context;
        private readonly ActionsService _actionsService;
        private readonly IFileManagerService _fileManagerService;

        public ProductsController(StoreContext context, ActionsService actionsService, IFileManagerService fileManagerService)
        {
            _context = context;
            _actionsService = actionsService;
            _fileManagerService = fileManagerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts()
        {
            await _actionsService.AddAction("Get products", "Products");
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();
            return Ok(productsDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            await _actionsService.AddAction("Get products by id", "Products");
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.IdProduct == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDTO = new ProductDTO
            {
                IdProduct = product.IdProduct,
                ProductName = product.NameProduct,
                Price = product.Price,
                DateUp = product.DateUp,
                Discontinued = product.Discontinued,
                PhotoUrl = product.PhotoUrl,
                CategoryName = product.Category.NameCategory
            };

            return Ok(productDTO);
        }

        [HttpGet("orderNameProduct/{desc}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsOrderName(bool desc)
        {
            await _actionsService.AddAction("Get products by order (name)", "Products");
            List<ProductDTO> productsDTO = new List<ProductDTO>();

            List<Product> products;
            if (desc)
            {
                products = await _context.Products
                    .OrderBy(x => x.NameProduct)
                    .Include(p => p.Category)
                    .ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .OrderByDescending(x => x.NameProduct)
                    .Include(p => p.Category)
                    .ToListAsync();
            }

            productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("nameProduct/contains/{text}")]
        public async Task<ActionResult<List<ProductDTO>>> GetNameProduct(string text)
        {
            await _actionsService.AddAction("Get products containing (name)", "Products");
            var products = await _context.Products
                .Where(x => x.NameProduct.Contains(text))
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("price/between")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByPrices([FromQuery] decimal min, [FromQuery] decimal max)
        {
            await _actionsService.AddAction("Get products with a price between", "Products");
            var products = await _context.Products
                .Where(x => x.Price > min && x.Price < max)
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("pagination/{page?}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsPagination(int page = 1)
        {
            await _actionsService.AddAction("Get Paginated Products", "Products");
            int recordsPerPage = 5;
            var products = await _context.Products
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage)
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("pagination/{from}/{until}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFromUntil(int from, int until)
        {
            await _actionsService.AddAction("Get Paginated Products from|until", "Products");
            if (from < 1)
            {
                return BadRequest("The minimum must be greater than 0");
            }
            if (until < from)
            {
                return BadRequest("The maximum cannot be less than the minimum");
            }

            var products = await _context.Products
                .Skip(from - 1)
                .Take(until - from)
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                ProductName = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                CategoryName = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("productSale")]
        public async Task<ActionResult<IEnumerable<ProductSaleDTO>>> GetProductsAndPrices()
        {
            await _actionsService.AddAction("Get products and prices", "Products");
            var products = await _context.Products
                .Include(x => x.Category)
                .Select(x => new ProductSaleDTO
                {
                    ProductName = x.NameProduct,
                    Price = x.Price,
                    CategoryName = x.Category.NameCategory
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("productsGroupedByDiscontinued")]
        public async Task<ActionResult> GetProductsGroupedByDiscontinued()
        {
            await _actionsService.AddAction("Get products discontinued", "Products");
            var products = await _context.Products
                .Include(p => p.Category)
                .GroupBy(g => g.Discontinued)
                .Select(x => new
                {
                    Discontinued = x.Key,
                    Total = x.Count(),
                    Products = x.Select(p => new
                    {
                        p.IdProduct,
                        p.NameProduct,
                        p.Price,
                        p.DateUp,
                        p.Discontinued,
                        p.PhotoUrl,
                        p.Category.NameCategory
                    }).ToList()
                }).ToListAsync();

            return Ok(products);
        }

        // Deferred consultation
        [HttpGet("filter")]
        public async Task<ActionResult> GetMultipleFilter([FromQuery] ProductFilterDTO filterProducts)
        {
            /* AsQueryable allows us to build the filter step by step and execute it at the end. If we 
            convert it to a list (toListAsync) we do the rest of the filters in memory because toListAsync 
            already brings the data from the database server to the server's memory. Doing the filters in 
            memory is less efficient than doing them in a database. We build the filters dynamically and 
            until we do the ToListAsync we do not go to the database to get the information. */
            await _actionsService.AddAction("Get products with a multiple filter", "Products");
            var productsQueryable = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterProducts.ProductName))
            {
                productsQueryable = productsQueryable.Where(x => x.NameProduct.Contains(filterProducts.ProductName));
            }

            if (filterProducts.Discontinued)
            {
                productsQueryable = productsQueryable.Where(x => x.Discontinued);
            }

            if (filterProducts.CategoryId != 0)
            {
                productsQueryable = productsQueryable.Where(x => x.CategoryId == filterProducts.CategoryId);
            }

            var productsDTO = await productsQueryable
                .Select(p => new ProductDTO
                {
                    IdProduct = p.IdProduct,
                    ProductName = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl,
                    CategoryName = p.Category.NameCategory
                })
                .ToListAsync();

            return Ok(productsDTO);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostProduct(ProductInsertDTO product)
        {
            await _actionsService.AddAction("Add products", "Products");
            var newProduct = new Product()
            {
                NameProduct = product.ProductName,
                Price = product.Price,
                DateUp = DateOnly.FromDateTime(DateTime.Now),
                Discontinued = product.Discontinued,
                PhotoUrl = "",
                CategoryId = (int)product.CategoryId
            };

            if (product.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // We extract the image from the request
                    await product.Photo.CopyToAsync(memoryStream);
                    // We convert it to a byte array which is what the save method needs.
                    var content = memoryStream.ToArray();
                    // We need the extension to save the file
                    var extension = Path.GetExtension(product.Photo.FileName);
                    // We received the name of the file
                    // The Transient File Manager service instantiates the service and when it is no longer used it is destroyed.
                    newProduct.PhotoUrl = await _fileManagerService.SaveFile(content, extension, "img",
                        product.Photo.ContentType);
                }
            }

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Created("Product", new { product = newProduct });
        }

        [Authorize]
        [HttpPut("{idProduct:int}")]
        public async Task<IActionResult> PutProduct(int idProduct, [FromForm] ProductUpdateDTO product)
        {
            await _actionsService.AddAction("Update product", "Products");
            if (idProduct != product.IdProduct)
            {
                return BadRequest(new { message = "The product ID does not match the request body" });
            }

            var productUpdate = await _context.Products
                .AsTracking()
                .FirstOrDefaultAsync(p => p.IdProduct == idProduct);

            if (productUpdate == null)
            {
                return NotFound(new { message = "The product was not found" });
            }

            if (product.CategoryId != 0)
            {
                var category = await _context.Categories.FindAsync(product.CategoryId);
                if (category == null)
                {
                    return BadRequest(new { message = "The category provided does not exist" });
                }
                productUpdate.CategoryId = (int)product.CategoryId;
            }

            productUpdate.NameProduct = product.ProductName;
            productUpdate.Price = product.Price;
            productUpdate.DateUp = product.DateUp;
            productUpdate.Discontinued = product.Discontinued;

            if (product.Photo != null)
            {
                using var memoryStream = new MemoryStream();
                await product.Photo.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(product.Photo.FileName);
                var contentType = product.Photo.ContentType;
                var routeImage = await _fileManagerService.SaveFile(content, extension, "img", contentType);
                productUpdate.PhotoUrl = routeImage;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating product", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _actionsService.AddAction("Delete product", "Products");
            var product = await _context.Products.FirstOrDefaultAsync(x => x.IdProduct == id);

            if (product is null)
            {
                return NotFound();
            }

            await _fileManagerService.DeleteFile(product.PhotoUrl, "img");
            _context.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}