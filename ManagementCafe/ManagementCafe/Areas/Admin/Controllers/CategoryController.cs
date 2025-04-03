using ManagementCafe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();
        // GET: CategoryController
        public ActionResult Index()
        {
            var listCategory = db.Categories.ToList();
            return View(listCategory);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            var category = db.Categories.Find(id);
            return View(category);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            if (string.IsNullOrWhiteSpace(collection["Name"]))
            {
                ViewBag.ErrorMessage = "Tên sản phẩm không được để trống.";
                return View();
            }

            try
            {
                int id = db.Categories.Count() + 1;
                var category = new Category()
                {
                    CateId = id,
                    Name = collection["Name"].ToString(),
                };
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi lưu dữ liệu.";
                return View();
            }
        }


        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var category = db.Categories.Find(id);
                if (category == null)
                {
                    return NotFound();
                }

                // Kiểm tra nếu giá trị mới khác với giá trị cũ và không được rỗng
                string newName = collection["Name"].ToString().Trim();
                if (!string.IsNullOrEmpty(newName) && !category.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
                {
                    category.Name = newName;
                    db.Update(category);
                    db.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            var cate = db.Categories.Find(id);
            if (cate == null)
            {
                return NotFound();
            }
            db.Categories.Remove(cate);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: CategoryController/Delete/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var category = db.Categories.Find(id);
                if (category == null)
                {
                    return NotFound();
                }
                db.Categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }
}
