using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();
        public IActionResult Index()
        {
            var listProduct = db.Products.ToList();
            return View(listProduct);
        }
        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = db.Categories.ToList();
            ViewBag.ListCategory = new SelectList(categories.Any() ? categories : new List<Category>(), "CateId", "Name");
            var namecate = db.Categories.Find(product.CateId);
            ViewBag.NameCate = namecate?.Name;
            return View(product);
        }

        public IActionResult Create()
        {
            var categories = db.Categories.ToList();
            ViewBag.ListCategory = new SelectList(categories.Any() ? categories : new List<Category>(), "CateId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection, IFormFile Image)
        {
            try
            {
                Console.WriteLine($"Image is null: {Image == null}");
                if (Image != null)
                {
                    Console.WriteLine($"Image FileName: {Image.FileName}, Length: {Image.Length}");
                }

                string name = collection["Name"].ToString();
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("Name", "Tên không được để trống.");
                }

                int cateId;
                if (!int.TryParse(collection["CateId"], out cateId) || cateId <= 0)
                {
                    ModelState.AddModelError("CateId", "Vui lòng chọn danh mục hợp lệ.");
                }
                else if (!db.Categories.Any(c => c.CateId == cateId))
                {
                    ModelState.AddModelError("CateId", "Danh mục không tồn tại.");
                }

                string description = collection["Description"].ToString();

                decimal price;
                if (!decimal.TryParse(collection["Price"], out price) || price < 0)
                {
                    ModelState.AddModelError("Price", "Giá phải là số hợp lệ và không âm.");
                }

                string status = collection["Status"].ToString();
                if (string.IsNullOrWhiteSpace(status))
                {
                    ModelState.AddModelError("Status", "Trạng thái không được để trống.");
                }

                string imageFileName = null;
                if (Image != null && Image.Length > 0)
                {
                    var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/product");
                    if (!Directory.Exists(imagesDirectory))
                    {
                        Directory.CreateDirectory(imagesDirectory);
                    }
                    var fileName = Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine(imagesDirectory, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    imageFileName = fileName;
                }
                else
                {
                    ModelState.AddModelError("Image", "Hình ảnh không được để trống.");
                }

                int productId = db.Products.Any() ? db.Products.Max(p => p.ProductId) + 1 : 1;
                if (db.Products.Any(p => p.ProductId == productId))
                {
                    ModelState.AddModelError("ProductId", "ID sản phẩm đã tồn tại.");
                }

                if (!ModelState.IsValid)
                {
                    var categories = db.Categories.ToList();
                    ViewBag.ListCategory = new SelectList(categories.Any() ? categories : new List<Category>(), "CateId", "Name");
                    ViewBag.Name = name;
                    ViewBag.Description = description;
                    ViewBag.Price = collection["Price"];
                    ViewBag.Status = status;
                    return View();
                }

                Product newProduct = new Product
                {
                    ProductId = productId,
                    Name = name,
                    Description = description,
                    Price = price,
                    Image = imageFileName,
                    Status = status,
                    CateId = cateId
                };

                db.Products.Add(newProduct);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu sản phẩm: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
                var categories = db.Categories.ToList();
                ViewBag.ListCategory = new SelectList(categories.Any() ? categories : new List<Category>(), "CateId", "Name");
                ViewBag.Name = collection["Name"].ToString();
                ViewBag.Description = collection["Description"].ToString();
                ViewBag.Price = collection["Price"].ToString();
                ViewBag.Status = collection["Status"].ToString();
                return View();
            }
        }

        public IActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = db.Categories.ToList();
            ViewBag.ListCategory = new SelectList(categories, "CateId", "Name", product.CateId);

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(IFormCollection form, IFormFile? ImageFile)
        {
            int productId = int.Parse(form["ProductId"].ToString());
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = form["Name"].ToString();
            product.Description = form["Description"];

            decimal price;
            string priceString = new string(form["PriceDisplay"].ToString().Where(char.IsDigit).ToArray());
            if (!decimal.TryParse(priceString, out price))
            {
                return BadRequest("Invalid price format");
            }
            product.Price = price;
            product.Status = form["Status"];
            product.CateId = int.Parse(form["CateId"].ToString());
            string? currentImage = form["CurrentImage"];

            // Thư mục lưu ảnh
            var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/product");

            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(currentImage))
                {
                    var oldImagePath = Path.Combine(imageFolderPath, currentImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu ảnh mới
                var fileName = Path.GetFileName(ImageFile.FileName);
                var newImagePath = Path.Combine(imageFolderPath, fileName);

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                product.Image = fileName; // Cập nhật ảnh mới vào product
            }
            else
            {
                product.Image = currentImage; // Giữ ảnh cũ nếu không có ảnh mới
            }

            db.Products.Update(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        //


        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}
