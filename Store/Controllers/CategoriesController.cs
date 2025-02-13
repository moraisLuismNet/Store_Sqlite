using Store.DTOs;
using Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly StoreContext _context;

        public CategoriesController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            var result = categories.Select(c => new CategoryDTO
            {
                IdCategory = c.IdCategory,
                NameCategory = c.NameCategory,
                Products = c.Products.Select(p => new ProductDTO
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryItemDTO>> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == id);

            if (category == null)
                return NotFound();

            var result = new CategoryItemDTO
            {
                NameCategory = category.NameCategory
            };

            return Ok(result);
        }

        [HttpGet("orderNameCategory/{desc}")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesOrderName(bool desc)
        {
            List<Category> categories;

            if (desc)
            {
                categories = await _context.Categories
                                           .Include(c => c.Products)
                                           .OrderBy(x => x.NameCategory)
                                           .ToListAsync();
            }
            else
            {
                categories = await _context.Categories
                                           .Include(c => c.Products)
                                           .OrderByDescending(x => x.NameCategory)
                                           .ToListAsync();
            }

            return Ok(categories);
        }

        [HttpGet("nameCategory/contains/{text}")]
        public async Task<ActionResult<List<CategoryDTO>>> GetNameCategory(string text)
        {
            var categories = await _context.Categories
                .Where(x => x.NameCategory.Contains(text))
                .Include(x => x.Products)
                .ToListAsync();

            var categoriesDTO = categories.Select(c => new CategoryDTO
            {
                IdCategory = c.IdCategory,
                NameCategory = c.NameCategory,
                Products = c.Products.Select(p => new ProductDTO
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl
                }).ToList()
            }).ToList();

            return categoriesDTO;
        }

        [HttpGet("paginacion/{page?}")]
        public async Task<ActionResult> GetCategoriesPagination(int page = 1)
        {
            int recordsPerPage = 2;

            var totalCategories = await _context.Categories.CountAsync();

            var categories = await _context.Categories
                .Include(x => x.Products)
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage)
                .ToListAsync();

            var categoriesDTO = categories.Select(c => new CategoryDTO
            {
                IdCategory = c.IdCategory,
                NameCategory = c.NameCategory,
                Products = c.Products.Select(p => new ProductDTO
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl
                }).ToList()
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalCategories / (double)recordsPerPage);

            return Ok(new { categories = categoriesDTO, totalPages });
        }


        [HttpGet("pagination/{from}/{until}")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesFromUntil(int from, int until)
        {
            if (from < 1)
            {
                return BadRequest("The minimum must be greater than 0");
            }
            if (until < from)
            {
                return BadRequest("The maximum cannot be less than the minimum");
            }

            var categories = await _context.Categories
                .Include(c => c.Products)
                .Skip(from - 1)
                .Take(until - from + 1)
                .ToListAsync();

            var categoriesDTO = categories.Select(c => new CategoryDTO
            {
                IdCategory = c.IdCategory,
                NameCategory = c.NameCategory,
                Products = c.Products.Select(p => new ProductDTO
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.NameProduct,
                    Price = p.Price,
                    DateUp = p.DateUp,
                    Discontinued = p.Discontinued,
                    PhotoUrl = p.PhotoUrl
                }).ToList()
            }).ToList();

            return Ok(categories);
        }

        [HttpGet("categoriesProductsSelect/{id:int}")]
        public async Task<ActionResult<Category>> GetCategoriesProductsSelect(int id)
        {
            var category = await (from x in _context.Categories
                                   select new CategoryProductDTO
                                   {
                                       IdCategory = x.IdCategory,
                                       NameCategory = x.NameCategory,
                                       TotalProducts = x.Products.Count(),
                                       Products = x.Products.Select(y => new ProductItemDTO
                                       {
                                           IdProduct = y.IdProduct,
                                           NameProduct = y.NameProduct
                                       }).ToList(),
                                   }).FirstOrDefaultAsync(x => x.IdCategory == id);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult> PostCategory(CategoryInsertDTO category)
        {
            var newCategory = new Category()
            {
                NameCategory = category.NameCategory
            };

            await _context.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return Created("Category", new { category = newCategory });
        }

        [HttpPut("{idCategory:int}")]
        public async Task<IActionResult> PutCategory(int idCategory, [FromBody] CategoryUpdateDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idCategory != category.IdCategory)
            {
                return BadRequest(new { message = "The route ID does not match the body ID" });
            }

            var categoryUpdate = await _context.Categories
                .AsTracking()
                .FirstOrDefaultAsync(x => x.IdCategory == idCategory);

            if (categoryUpdate == null)
            {
                return NotFound(new { message = "The category was not found" });
            }

            categoryUpdate.NameCategory = category.NameCategory;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating category", details = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var thereAreProducts = await _context.Products.AnyAsync(x => x.CategoryId == id);
            if (thereAreProducts)
            {
                return BadRequest("There are related products");
            }
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.IdCategory == id);

            if (category is null)
            {
                return NotFound();
            }

            _context.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}