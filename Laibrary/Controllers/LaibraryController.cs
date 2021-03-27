using Laibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laibrary.Controllers
{
    public class LaibraryController : Controller
    {
        // GET: Laibrary
        DBContext db = new DBContext();
       
        public ActionResult Index()
        {
           List<Book> book= db.Books.ToList();
            return View(book);
        }
        public ActionResult Show_Book(int id)
        {
            Book book = db.Books.Where(n => n.Isbn == id).FirstOrDefault();

            if (book.Count != 0)
            {
                Session["Count"] = null;
            }
            if (book.Count != book.CurrentCount)
            {
                Session["CurrentCount"] = null;
            }

            return View(book);
        }
        public ActionResult Borrow_Book(int id)
        {
            Session["BookID"] = id;
            Book book = db.Books.Where(n => n.Isbn == id).FirstOrDefault();
            
            if (book.Count == 0)
            {
                Session["Count"] = "this book is be not found";
                return RedirectToAction("Show_Book", new { id = id });
            }


            return View(new User());
        }

        [HttpPost]
        public ActionResult Borrow_Book(User user)
        {
            int id = (int)Session["BookID"];
            User use = db.Users.Where(n => n.Id == user.Id).SingleOrDefault();
            if (use == null)
            {
                User u = new User()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    E_Mail = user.E_Mail,
                    Address = user.Address,
                };
                Activity ac = new Activity()
                {
                    UserId = user.Id,
                    BookId = (int)Session["BookID"],
                    BorrowDate = DateTime.Now
                };
                Book book = db.Books.Where(n => n.Isbn == id).SingleOrDefault();
                book.Count -= 1;
                db.Users.Add(u);
                db.Activities.Add(ac);
                db.SaveChanges();
            }
            else
            {
                Book book = db.Books.Where(n => n.Isbn == id).SingleOrDefault();
                book.Count -= 1;
                Activity actv = db.Activities.Where(n => n.UserId == user.Id && n.BookId == id).SingleOrDefault();
                if (actv == null)
                {
                    Activity ac = new Activity()
                    {
                        UserId = user.Id,
                        BookId = (int)Session["BookID"],
                        BorrowDate = DateTime.Now
                    };
                    db.Activities.Add(ac);
                    db.SaveChanges();
                }
                else
                {
                    actv.BorrowDate = DateTime.Now;
                    db.SaveChanges();
                }


            }

            return RedirectToAction("Index");
        }
        public ActionResult Retrieval_Book(int id)
        {
            Session["BookID"] = id;
            Book book = db.Books.Where(n => n.Isbn == id).FirstOrDefault();
            Session["CurrentCount"] = null;
            if (book.Count == book.CurrentCount)
            {
                Session["CurrentCount"] = "this book is be found";
                return RedirectToAction("Show_Book", new { id = id });
            }
 
                return View(new User());
            
        }
        [HttpPost]
        public ActionResult Retrieval_Book(User user)
        {
            int id = (int)Session["BookID"];
           Activity ac= db.Activities.Where(n => n.UserId == user.Id && n.BookId == id).SingleOrDefault();
            if (ac == null)
            {
                Session["error"] = "You Are Not Borrowed this book";
                return View("Submit_View");
            }
            else
            {
                ac.RetrievalDate = DateTime.Now;
                Book book = db.Books.Where(n => n.Isbn == id).SingleOrDefault();
                book.Count += 1;
                db.SaveChanges();
                return View("Submit_View");

            }

            //return RedirectToAction("Index");
        }
        public ActionResult Submit_View()
        {
            
            return View();

        }
        
    }
}