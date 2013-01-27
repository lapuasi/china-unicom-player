using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShowRommSys.Client.Web.Models;
using System.Web.Security;
using System.Text;
using System.IO;

namespace ShowRommSys.Client.Web.Controllers
{
    public class PadController : Controller
    {
        //
        // GET: /Pad/
        public IFormsAuthenticationService FormsService { get; set; }
        private static readonly ShowRoomSysEntities DBContext = new ShowRoomSysEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogOn()
        {
            return View();
        }

        public JsonResult PlayItems(int? ID, string name)
        {
            if (ID.HasValue)
            {
                var lists = DBContext.ItemsResources.Where(e => e.ItemId == ID).ToList();
                //<div class='button'><a href='javascript:;' class='button' id='btnNext'>开始演示</a></div>
                StringBuilder html = new StringBuilder(@"<div class='top'>
                    <div class='title one right'>
                        " + name + @"</div>
                </div>
                <div class='content'>
                    
                    <div class='label'>
                        <h2>
                            共" + lists.Count + @"个媒体</h2>
                    </div>
                    <div class='playlist'>
                        <ul>");
                foreach (var v in lists)
                {
                    var resource = DBContext.Resources.FirstOrDefault(e => e.Id == v.ResourceId);
                    var device = DBContext.Device.FirstOrDefault(e => e.Id == v.DeviceId);
                    string str = "";
                    string ext = Path.GetExtension(resource.Uri).ToLower();
                    System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"^(http|HTTP|https|HTTPS)://([\w-]+\.)*[\w-]+(:\d+)?(/[\u4e00-\u9fa5\w- ./?%&=]*)?$");
                    if (re.IsMatch(resource.Uri))
                        ext = ".url";
                    switch (ext)
                    {
                        case ".ppt":
                        case ".pptx":
                            str = "filetype filetype-ppt";
                            break;
                        case ".doc":
                        case ".docx":
                            str = "filetype filetype-doc";
                            break;
                        case ".xls":
                        case ".xlsx":
                            str = "filetype filetype-xls";
                            break;
                        case ".avi":
                        case ".wmv":
                            str = "filetype filetype-avi";
                            break;
                        case ".jpg":
                        case ".png":
                        case ".bmp":
                            str = "filetype filetype-img";
                            break;
                        case ".pdf":
                            str = "filetype filetype-pdf";
                            break;
                        case ".url":
                            str = "filetype filetype-url";
                            break;
                        case ".exe":
                            str = "filetype filetype-exe";
                            break;
                        default:
                            break;
                    }
                    html.Append(@"
                            <li>
                                <p class='subject'>
                                    <span class='" + str + @"'></span><span class='filename'>" + resource.Name + "</span></p>");
                    html.Append("<p class='control'>");
                    string[] arrExts = {".jpg",".png",".bmp",".url",".exe"};
                    if (!arrExts.Contains(ext))
                        html.Append("<a href='javascript:OptionPlay(1,\"" + device.MAC + "\");' class='rw'>RW</a>");
                    html.Append("<a href='javascript:StartPlay(\"" + device.MAC + "\",\"" + resource.Uri + "\");' class='play'>开始</a>");
                    if (!arrExts.Contains(ext))
                        html.Append("<a href='javascript:OptionPlay(2,\"" + device.MAC + "\");' class='rw'>FF</a>");
                    html.Append("</li>");//<a href='javascript:OptionPlay(8,\"" + device.MAC + "\");' class='ff'>全屏</a>

                }
                html.Append("</ul></div></div>");
                string str1 = html.ToString();
                return Json(html.ToString(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpPost]
        public ActionResult LogOn(Users user)
        {
            var q = from u in DBContext.Users
                    join r in DBContext.Roles on u.RoleId equals r.Id
                    where u.Number == user.Number && u.Password==user.Password&&r.Name=="讲解员"
                    select u.Id;
            int id = q.FirstOrDefault();
            if (id != 0)
            {
                FormsAuthentication.SetAuthCookie(id.ToString(), false);
                return RedirectToAction("PlayList", "pad");
            }
            else
            {
                ViewData["Error"] = "用户名或密码错误，请重新填写！";
                return View();
            }

        }

        public ActionResult PlayList()
        {
            var now = DateTime.Now;
            return View(DBContext.Natures.Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month && e.Date.Day == now.Day));
        }
        public ActionResult PlayDetails(int id)
        {
            ViewData["id"] = id;
            return View();
        }
    }
}
