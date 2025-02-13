using Store.DTOs;
using Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DTOs;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();
            return Ok(productsDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
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
                NameProduct = product.NameProduct,
                Price = product.Price,
                DateUp = product.DateUp,
                Discontinued = product.Discontinued,
                PhotoUrl = product.PhotoUrl,
                NameCategory = product.Category.NameCategory
            };

            return Ok(productDTO);
        }

        [HttpGet("orderNameProduct/{desc}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsOrderName(bool desc)
        {
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
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("nameProduct/contains/{text}")]
        public async Task<ActionResult<List<ProductDTO>>> GetNameProduct(string text)
        {
            var products = await _context.Products
                .Where(x => x.NameProduct.Contains(text))
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("price/between")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByPrices([FromQuery] decimal min, [FromQuery] decimal max)
        {
            var products = await _context.Products
                .Where(x => x.Price > min && x.Price < max)
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("pagination/{page?}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsPagination(int page = 1)
        {
            int recordsPerPage = 5;
            var products = await _context.Products
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage)
                .Include(p => p.Category)
                .ToListAsync();

            var productsDTO = products.Select(p => new ProductDTO
            {
                IdProduct = p.IdProduct,
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("pagination/{from}/{until}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFromUntil(int from, int until)
        {
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
                NameProduct = p.NameProduct,
                Price = p.Price,
                DateUp = p.DateUp,
                Discontinued = p.Discontinued,
                PhotoUrl = p.PhotoUrl,
                NameCategory = p.Category.NameCategory
            }).ToList();

            return Ok(productsDTO);
        }

        [HttpGet("productSale")]
        public async Task<ActionResult<IEnumerable<ProductSaleDTO>>> GetProductsAndPrices()
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .Select(x => new ProductSaleDTO
                {
                    NameProduct = x.NameProduct,
                    Price = x.Price,
                    NameCategory = x.Category.NameCategory
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("productsGroupedByDiscontinued")]
        public async Task<ActionResult> GetProductsGroupedByDiscontinued()
        {
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
            var productsQueryable = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterProducts.NameProduct))
            {
                productsQueryable = productsQueryable.Where(x => x.NameProduct.Contains(filterProducts.NameProduct));
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
                    NameProduct = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl,
                    NameCategory = p.Category.NameCategory
                })
                .ToListAsync();

            return Ok(productsDTO);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(ProductInsertDTO product)
        {
            var newProduct = new Product()
            {
                NameProduct = product.NameProduct,
                Price = product.Price,
                DateUp = product.DateUp,
                Discontinued= product.Discontinued,
                PhotoUrl = product.PhotoUrl,
                CategoryId = (int)product.CategoryId
            };

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Created("Product", new { product = newProduct });
        }

        [HttpPut("{idProduct:int}")]
        public async Task<IActionResult> PutProduct(int idProduct, [FromBody] ProductUpdateDTO product)
        {
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

            productUpdate.NameProduct = product.NameProduct;
            productUpdate.Price = product.Price;
            productUpdate.DateUp = product.DateUp;
            productUpdate.Discontinued = product.Discontinued;
            productUpdate.PhotoUrl = product.PhotoUrl;
            productUpdate.CategoryId = (int)product.CategoryId;

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


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.IdProduct == id);

            if (product is null)
            {
                return NotFound();
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}