using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using MyFamily.Models;

namespace MyFamily.Controllers
{
    public class MemberController : Controller
    {
        AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MemberController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<MemberViewModel> memberViewModelList = new List<MemberViewModel>();
            var memberList = (from m in _db.Members
                              select new
                              {
                                  m.MemberId,
                                  m.Name,
                                  m.Age,
                                  m.ImagePath
                              }).ToList();
            foreach(var item in memberList)
            {
                MemberViewModel objMvm = new MemberViewModel();
                objMvm.MemberId = item.MemberId;
                objMvm.Name = item.Name;
                objMvm.Age = item.Age;
                objMvm.ImagePath = item.ImagePath;
                memberViewModelList.Add(objMvm);
            }
            return View(memberViewModelList);
        }
        private string UploadFile(MemberViewModel model)
        {
            string filePath = null;
            if(model.Image != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                filePath = Guid.NewGuid().ToString() + " " + model.Image.FileName;
                string path = Path.Combine(uploadsFolder, filePath);
                using(var fileStream = new FileStream(path, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                };
            }
            return filePath;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(MemberViewModel model)
        {
            ModelState.Remove("MemberId");
            if (ModelState.IsValid)
            {
                string filepath = UploadFile(model);
                Member member = new Member
                {
                    MemberId = model.MemberId,
                    Name = model.Name,
                    Age = model.Age,
                    ImagePath = filepath
                };
                _db.Add(member);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            Member data = _db.Members.Find(id);
            MemberViewModel model = new MemberViewModel();
            if(data != null)
            {
                model.MemberId = data.MemberId;
                model.Name = data.Name;
                model.Age = data.Age;
                model.ImagePath = data.ImagePath;
            }
            return View("Create", model);            
        }
        [HttpPost]
        public IActionResult Edit(MemberViewModel model) 
        {
            if (ModelState.IsValid)
            {
                Member member = new Member
                {
                    MemberId = model.MemberId,
                    Name = model.Name,
                    Age = model.Age,
                    ImagePath = model.ImagePath
                };
                if(model.Image != null)
                {
                    string filePath = UploadFile(model);
                    member.ImagePath = filePath;
                };
                _db.Members.Update(member);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", model);
        }
        public IActionResult Delete(int id)
        {
            Member member = _db.Members.Find(id);
            if(member != null)
            {
                _db.Members.Remove(member);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
