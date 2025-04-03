using ManagementCafe.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagementCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartyTableController : Controller
    {
        ManagementCafeContext db = new ManagementCafeContext();
        public IActionResult Index()
        {
            var listTable = db.PartyTables.ToList();
            return View(listTable);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                string name = collection["Name"].ToString();
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("Name", "Tên không được để trống.");
                }
                var table = new PartyTable()
                {
                    TableId = int.Parse(collection["TableId"].ToString()),
                    Status = collection["Status"].ToString(),
                    Capacity = int.Parse(collection["Capacity"].ToString())
                };
                db.PartyTables.Add(table);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }

        }

        public IActionResult Edit(int id)
        {
            var table = db.PartyTables.Find(id);
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var id = int.Parse(collection["TableId"].ToString());
                var table = db.PartyTables.Find(id);
                if (table == null)
                {
                    return NotFound();
                }

                // Kiểm tra nếu giá trị mới khác với giá trị cũ và không được rỗng
                string newStatus = collection["Status"].ToString().Trim();
                int newCapacity = int.Parse(collection["Capacity"].ToString());
                if (newStatus != table.Status)
                {
                    table.Status = newStatus;
                }
                if (newCapacity != table.Capacity)
                {
                    table.Capacity = newCapacity;
                }
                db.PartyTables.Update(table);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public IActionResult Delete(int id)
        {
            var table = db.PartyTables.Find(id);
            if (table == null)
            {
                return NotFound();
            }
            db.PartyTables.Remove(table);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
