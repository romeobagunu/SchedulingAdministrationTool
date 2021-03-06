using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAT.DATA.EF;
using SAT.MVC.UI.Utilities;

namespace SAT.MVC.UI.Controllers
{
    public class StudentsController : Controller
    {
        private SchedulingAdministrationToolEntities db = new SchedulingAdministrationToolEntities();

        // GET: Students
        [Authorize(Roles = "Admin,Scheduling")]
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.StudentStatus);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        [Authorize(Roles = "Admin,Scheduling")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.SSID = new SelectList(db.StudentStatuses, "SSID", "SSName");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "StudentID,FirstName,LastName,Major,Address,City,State,ZipCode,Phone,Email,PhotoURL,SSID")] Student student, HttpPostedFileBase PhotoURL)
        {
            if (ModelState.IsValid)
            {

                #region File Upload

                string file = "placeholder.jpg";

                if (PhotoURL != null)
                {
                    file = PhotoURL.FileName;
                    string extension = file.Substring(file.LastIndexOf("."));
                    string[] goodextension = { ".jpeg", ".jpg", ".png", ".gif" };
                    if (goodextension.Contains(extension.ToLower()) && PhotoURL.ContentLength <= 4194304)
                    {
                        file = Guid.NewGuid() + extension;
                        string savePath = Server.MapPath("~/Content/images/students/");
                        Image ConvertedImage = Image.FromStream(PhotoURL.InputStream);
                        int maxImageSize = 500;
                        int maxThumbSize = 100;
                        ImageUtility.ResizeImage(savePath, file, ConvertedImage, maxImageSize, maxThumbSize);
                    }

                    student.PhotoURL = file;

                }

                #endregion

                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SSID = new SelectList(db.StudentStatuses, "SSID", "SSName", student.SSID);
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.SSID = new SelectList(db.StudentStatuses, "SSID", "SSName", student.SSID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "StudentID,FirstName,LastName,Major,Address,City,State,ZipCode,Phone,Email,PhotoURL,SSID")] Student student, HttpPostedFileBase PhotoURL)
        {
            if (ModelState.IsValid)
            {
                #region File Upload

                string file = student.PhotoURL;

                if (PhotoURL != null)
                {
                    file = PhotoURL.FileName;

                    string extension = file.Substring(file.LastIndexOf("."));
                    string[] goodextension = { ".jpeg", ".jpg", ".png", ".gif" };
                    if (goodextension.Contains(extension.ToLower()) && PhotoURL.ContentLength <= 4194304)
                    {
                        file = Guid.NewGuid() + extension;

                        #region Resizing

                        string savePath = Server.MapPath("~/Content/images/students/");
                        Image ConvertedImage = Image.FromStream(PhotoURL.InputStream);
                        int maxImageSize = 500;
                        int maxThumbSize = 100;
                        ImageUtility.ResizeImage(savePath, file, ConvertedImage, maxImageSize, maxThumbSize);

                        #endregion

                        if (student.PhotoURL != null && student.PhotoURL != "placeholder.jpg")
                        {
                            string path = Server.MapPath("~/Content/images/students/");
                            ImageUtility.Delete(path, student.PhotoURL);
                        }

                        student.PhotoURL = file;

                    }
                }
                #endregion

                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SSID = new SelectList(db.StudentStatuses, "SSID", "SSName", student.SSID);
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
