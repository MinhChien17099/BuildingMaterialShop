using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using BuildingMaterialShop.ApiModels.ProductViewModel;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Util.Store;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "MaterialShop";
        public static IWebHostEnvironment _webhostEnvieronment;

        public ProductsController(BuildingMaterialsShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webhostEnvieronment = webHostEnvironment;

        }

        // GET: Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(pro => pro.WareHouses)
                                            .Include(pro => pro.Category)
                                            .ToListAsync();

        }


        // GET: Products/5
        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProductDetails(string productId)
        {
            var product = await _context.Products.Include(pro => pro.WareHouses)
                                                .Include(pro => pro.Category)
                                                .Include(pro => pro.Supplies).ThenInclude(pro => pro.Supplier)
                                                .Where(pro => pro.ProductId == productId).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] InsertProductViewModel model)
        {
            if (String.IsNullOrEmpty(model.productId))
            {
                return Ok("Mã sản phẩm không được để trống.");
            }

            if (!CategoryExists(model.categoryId))
            {
                return NotFound();
            }
            foreach (var item in model.supplies)
            {
                if (!SupplierExists(item))
                    return NotFound();

            }
            if (model.Image == null)
            {
                return Ok("Vui lòng chọn hình ảnh đính kèm.");
            }
            if (model.Image.ContentType != "image/jpeg" && model.Image.ContentType != "image/png")
            {
                return Ok("Chỉ chấp nhận định dạng jpeg hoặc png.");
            }
            if (model.Image.Length > 1048576)
            {
                return Ok("Kích thước không vượt quá 1mb");
            }

            string path = _webhostEnvieronment.WebRootPath + "\\uploads\\";

            try
            {
                if (model.Image.Length > 0)
                {

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream file = System.IO.File.Create(path + model.Image.FileName))
                    {
                        model.Image.CopyTo(file);
                        file.Flush();
                    }
                }
            }
            catch (Exception e)
            {

                return Ok(e.Message);
            }

            UserCredential credential;

            credential = GetCredentials();

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //Id của folder muốn upload
            string folderid = "1iCNelsHCSLGKQL9XRJwMu9nUHZURFgR8";

            var filePath = path + model.Image.FileName;

            string fileId = "1paIUZyOqIW5ZllHXE0b6r-kZWJHQMQYR";
            try
            {
                fileId = UploadImage(filePath, service, folderid);
                model.images = fileId;

            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }

            var product = model.ToProduct();

            WareHouse ware = new WareHouse();
            ware.Date = DateTime.Now;
            ware.ProductId = product.ProductId;
            ware.Quantity = 0;

            product.WareHouses.Add(ware);

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductDetails", new { productId = product.ProductId }, product);
        }

        // PUT: Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
        private string UploadImage(string path, DriveService service, string folderUpload)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = Path.GetFileName(path);
            fileMetadata.MimeType = "image/*";

            fileMetadata.Parents = new List<string>
            {
                folderUpload
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/*");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            return file.Id;

        }

        private UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream = new FileStream("bin\\Debug\\net5.0\\ref\\credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, "token.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine(string.Format("Credential file saved to: " + credPath));
            }

            return credential;
        }
        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        private bool CategoryExists(string categoryId)
        {
            return _context.Categories.Any(e => e.CategoryId == categoryId);
        }
        private bool SupplierExists(string supplierId)
        {
            return _context.Suppliers.Any(e => e.SupplierId == supplierId);
        }
    }
}
