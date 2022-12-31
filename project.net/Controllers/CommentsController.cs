﻿using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.net.Data;
using project.net.Models;

namespace project.net.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<AppUser> userManager;

        public CommentsController(
            ApplicationDbContext context,
            IWebHostEnvironment host,
            UserManager<AppUser> userManager)
        {
            this.db = context;
            this.webHostEnvironment = host;
            this.userManager = userManager;
        }


        [Authorize]
        [HttpPost]
        [Route("/post-comment/<bookmarkId:int>")]
        public IActionResult Post(Bookmark bookmark, int bookmarkId)
        {
            //With form
            if (bookmark.AddedComment?.Content == null)
                return RedirectToAction("Index", "Bookmarks", new { bookmarkId = bookmark.Id });

            Comment comment = new()
            {
                UserId = userManager.GetUserId(User),
                BookmarkId = bookmarkId,
                Content = bookmark.AddedComment.Content,
                Image = bookmark.AddedComment.Image,
                CreatedAt = DateTime.Now
            };
            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Index", "Bookmarks", new { bookmarkId = (int?)bookmarkId });
        }

        [Authorize]
        [HttpPost]
        [Route("/new-comment")]
        public IActionResult New([FromBody]Comment comment)
        {
            var userId = userManager.GetUserId(User);
            comment.UserId = userId;
            comment.CreatedAt = DateTime.Now;
            db.Comments.Add(comment);
            db.SaveChanges();
            return Json(new { content = comment.Content });
        }

        [Authorize]
        [HttpPost]
        [Route("/edit-comment")]
        public IActionResult Edit([FromBody]Comment receivedComment)
        {
            //todo ii dai voie si adminului
            //todo uita-te si in view sa ii afisezi butoanele

            var userId = userManager.GetUserId(User);
            var comment = db.Comments.FirstOrDefault(c => c.Id == receivedComment.Id);

            //aici vom testa si daca nu e admin
            if (comment == null || comment.UserId != userId)
                return NotFound();

            comment.Content = receivedComment.Content;
            db.Comments.Update(comment);
            db.SaveChanges();

            return Json(new { commentId =  receivedComment.Id });
        }

        [Authorize]
        [HttpPost]
        [Route("/delete-comment")]
        public IActionResult Delete([FromBody]Comment receivedComment)
        {
            //todo ii dai voie si adminului
            //todo uita-te si in view sa ii afisezi butoanele

            var userId = userManager.GetUserId(User);
            var comment = db.Comments.FirstOrDefault(c => c.Id == receivedComment.Id);

            //aici vom testa si daca nu e admin
            if (comment == null || comment.UserId != userId)
                return NotFound();

            db.Comments.Remove(comment);
            db.SaveChanges();

            return Json(new { commentId =  receivedComment.Id });
        }
    }
}
