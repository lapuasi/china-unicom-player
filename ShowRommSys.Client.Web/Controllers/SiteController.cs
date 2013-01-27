using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShowRommSys.Client.Web.Models;
using System.ServiceModel;
using ShowRommSys.Client.Web.Common;
using System.IO;
using Telerik.Web.Mvc;

namespace ShowRommSys.Client.Web.Controllers
{
    public class SiteController : Controller, ControlService.IMainControlCallback
    {
        private static readonly ShowRoomSysEntities DBContext = new ShowRoomSysEntities();
        //
        // GET: /Site/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Main()
        {
            return View();
        }
        public ActionResult Menu()
        {
            var items = FunctionMenu.GetFunctionMenus(int.Parse(this.User.Identity.Name));
            return View(items);
        }
        public ActionResult UserInfo()
        {
            string user = User.Identity.Name;
            return View();
        }
        public ActionResult Welcome()
        {
            return View();
        }
        #region list
        [HttpPost]
        public ActionResult AddList(ShowItemsList itemList)
        {
            itemList.CreateTime = DateTime.Now;
            DBContext.AddToShowItemsList(itemList);
            DBContext.SaveChanges();
            if (Request.Form["resourceid"] != null)
            {
                string[] resources = Request.Form["resourceid"].TrimEnd(',').Split(',');
                var delList = DBContext.ListResources.Where(e => e.ListID == itemList.Id).ToList();
                foreach (var v in delList)
                {
                    DBContext.ListResources.DeleteObject(v);
                }
                for (int i = 0; i < resources.Length; i++)
                {
                    if (resources[i] != string.Empty)
                    {
                        ListResources ir = new ListResources { ListID = itemList.Id, ResourceID = int.Parse(resources[i]) };
                        DBContext.AddToListResources(ir);
                    }
                }
                DBContext.SaveChanges();
            }
            return RedirectToAction("IndexList");
        }
        public ActionResult IndexList()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            var list = DBContext.ShowItemsList.ToList();
            return View(list);
        }
        public ActionResult DetailsList(int ID)
        {
            var list = DBContext.ShowItemsList.FirstOrDefault(e => e.Id == ID);
            var rs = DBContext.ListResources.Where(e => e.ListID == ID).ToList();
            string results = string.Empty;
            foreach(var v in rs)
            {
                results += DBContext.Resources.FirstOrDefault(e => e.Id == v.ResourceID).Name + System.Environment.NewLine; 
            }
            ViewData["Resources"] = results;
            return View(list);
        }
        public ActionResult PlayList(int ID)
        {
            return View();
        }
        public ActionResult AddList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var resources = DBContext.Resources.Select(e => e.Category).Distinct().ToList();
            foreach (var v in resources)
                list.Add(new SelectListItem { Text = v, Value = v });
            ViewData["ResourcesType"] = list;
            return View();
        }
        #endregion

        #region resource
        public ActionResult AddResources()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddResources(Resources rs)
        {
            if (ModelState.IsValid)
            {
                string type = Request.Form["Type"];
                string category = string.Empty;
                if (type == "2")
                    category = "链接";
                else if (type == "3")
                {
                    category = "应用程序";
                }
                else
                {
                    switch (Path.GetExtension(Request.Form["FilePath"]).ToLower().Replace(".", ""))
                    {
                        case "ppt":
                        case "pptx":
                            category = "幻灯";
                            break;
                        case "doc":
                        case "docx":
                            category = "文档";
                            break;
                        case "xls":
                        case "xlsx":
                            category = "Excel";
                            break;
                        case "avi":
                        case "wmv":
                            category = "视频";
                            break;
                        case "pdf":
                            category = "PDF";
                            break;
                        default:
                            category = "其它";
                            break;
                    }
                }
                Resources res = new Resources();
                res.Type = int.Parse(type);
                res.Approve = false;
                res.Remark = rs.Remark == null ? "" : rs.Remark;
                res.Uri = type == "1" ? Request.Form["FilePath"] : Request.Form["url"];
                if (type == "3")
                    res.Uri = Request.Form["exepath"];
                res.CreateTime = DateTime.Now;
                res.Category = category;
                res.Name = type == "1" ? Path.GetFileNameWithoutExtension(Request.Form["FilePath"]) : Request.Form["url"];
                if (type == "3")
                    res.Name = Path.GetFileNameWithoutExtension(Request.Form["exepath"]);
                DBContext.Resources.AddObject(res);
                DBContext.SaveChanges();
                return RedirectToAction("IndexResources");
            }
            return View();
        }
        public ActionResult IndexResources()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            return View(DBContext.Resources.ToList());
        }
        [HttpPost]
        public ActionResult ApproveResources(string ID)
        {
            if (ID == null)
                return null;
            List<int> result = new List<string>(ID.TrimEnd(',').Split(',')).ConvertAll(i => int.Parse(i));
            var resource = DBContext.Resources.Where(e => result.Contains(e.Id)==true);
            foreach (var r in resource)
            {
                r.Approve = true;
            }
            DBContext.SaveChanges();
            return null;
        }
        public ActionResult EditResources(int ID)
        {
            var resource = DBContext.Resources.FirstOrDefault(e => e.Id == ID);
            return View(resource);
        }
        [HttpPost]
        public ActionResult EditResources(Resources rs)
        {
            var resource = DBContext.Resources.SingleOrDefault(e => e.Id == rs.Id);
            string category = string.Empty;
            if (rs.Type == 2)
                category = "链接";
            else if (rs.Type == 1)
            {
                switch (Path.GetExtension(rs.Uri).ToLower().Replace(".", ""))
                {
                    case "ppt":
                    case "pptx":
                        category = "幻灯";
                        break;
                    case "doc":
                    case "docx":
                        category = "文档";
                        break;
                    case "xls":
                    case "xlsx":
                        category = "Excel";
                        break;
                    case "avi":
                    case "wmv":
                        category = "视频";
                        break;
                    default:
                        category = "其它";
                        break;
                }
            }
            else
                category = "应用程序";
            resource.Type = rs.Type;
            resource.Category = category;
            resource.Uri = rs.Uri;
            resource.Remark = rs.Remark;
            resource.Approve = rs.Approve;
            resource.CreateTime = rs.CreateTime;
            resource.Name = rs.Type == 2 ? rs.Uri : Path.GetFileNameWithoutExtension(rs.Uri);
            DBContext.SaveChanges();
            return RedirectToAction("IndexResources");
        }
        #endregion

        #region device
        public ActionResult AddDevice()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddDevice(Device device)
        {
            DBContext.Device.AddObject(device);
            DBContext.SaveChanges();
            return RedirectToAction("IndexDevice");
        }
        public ActionResult IndexDevice()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            var list = DBContext.Device.ToList();
            string macs = GetOnlineStatus().Replace(":","-");
            foreach (var v in list)
            {
                if (macs.IndexOf(v.MAC) >= 0)
                    v.Status = "在线";
                else
                    v.Status = "离线";
            }
            return View(list);
        }
        [NonAction]
        public string GetOnlineStatus()
        {
            try
            {
                var ctx = new InstanceContext(this);
                var svc = new ControlService.MainControlClient(ctx);
                svc.ClientCredentials.Windows.ClientCredential.UserName = System.Configuration.ConfigurationManager.AppSettings["user"];
                svc.ClientCredentials.Windows.ClientCredential.Password = System.Configuration.ConfigurationManager.AppSettings["pwd"];
                string result = svc.GetOnLineStatus();
                svc.ChannelFactory.Close();
                return result;
            }
            catch {
                return "";
            }
        }
        public ActionResult EditDevice(int ID)
        {
            var device = DBContext.Device.FirstOrDefault(e => e.Id == ID);
            return View(device);
        }
        public void DeleteDevice(string ID)
        {
            List<int> result = new List<string>(ID.TrimEnd(',').Split(',')).ConvertAll(i => int.Parse(i));
            var list = DBContext.Device.Where(e => result.Contains(e.Id)==true);
            foreach (var v in list)
                DBContext.DeleteObject(v);
            DBContext.SaveChanges();
        }
        [HttpPost]
        public ActionResult EditDevice(Device device)
        {
            var d = DBContext.Device.Where(e => e.Id == device.Id).SingleOrDefault();
            d.MAC = device.MAC;
            d.Name = device.Name;
            d.Remark = device.Remark;
            DBContext.SaveChanges();
            return RedirectToAction("IndexDevice");
        }
        #endregion

        #region Items
        public void DeleteItems(string ID)
        {
            List<int> result = new List<string>(ID.TrimEnd(',').Split(',')).ConvertAll(i => int.Parse(i));
            var list = DBContext.Items.Where(e => result.Contains(e.Id)==true);
            foreach (var v in list)
                DBContext.DeleteObject(v);
            DBContext.SaveChanges();
        }
        public ActionResult AddItems()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var resources = DBContext.Resources.Select(e => e.Category).Distinct().ToList();
            foreach (var v in resources)
                list.Add(new SelectListItem { Text = v, Value = v });
            ViewData["ResourcesType"] = list;
            List<SelectListItem> list1 = new List<SelectListItem>();
            var devices = DBContext.Device.ToList();
            foreach (var v in devices)
            {
                list1.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });
            }
            ViewData["Devices"] = list1;
            ViewData["ShowLists"] = DBContext.ShowItemsList.ToList().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() }).ToList<SelectListItem>();
            return View();
        }
        public JsonResult GetResources(string ID)
        {
            if (ID != null && ID != string.Empty)
            {
                var list = DBContext.Resources.Where(c => c.Category == ID).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        [HttpPost]
        public ActionResult AddItems(Items item)
        {
            item.CreateTime = DateTime.Now;
            DBContext.AddToItems(item);
            DBContext.SaveChanges();
            if (Request.Form["resourceid"] != null && Request.Form["diveceid"] != null)
            {
                string[] resources = Request.Form["resourceid"].TrimEnd(',').Split(',');
                string[] devices = Request.Form["diveceid"].TrimEnd(',').Split(',');
                string str = Request.Form["PlayName"];
                for (int i = 0; i < resources.Length; i++)
                {
                    if (resources[i] != string.Empty && devices[i] != string.Empty)
                    {
                        ItemsResources ir = new ItemsResources { ItemId = item.Id, ResourceId = int.Parse(resources[i]), DeviceId = int.Parse(devices[i])};
                        DBContext.AddToItemsResources(ir);
                    }
                }
                DBContext.SaveChanges();
            }
            return RedirectToAction("IndexItems");
        }
        public ActionResult EditItems(int ID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var resources = DBContext.Resources.Select(e => e.Category).Distinct().ToList();
            foreach (var v in resources)
                list.Add(new SelectListItem { Text = v, Value = v });
            ViewData["ResourcesType"] = list;
            List<SelectListItem> list1 = new List<SelectListItem>();
            var devices = DBContext.Device.ToList();
            foreach (var v in devices)
            {
                list1.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });
            }
            ViewData["Devices"] = list1;
            var item = DBContext.Items.FirstOrDefault(e=>e.Id==ID);
            ViewData["ListName"] = DBContext.ShowItemsList.FirstOrDefault(e => e.Id == item.ListId).Name;
            return View(item);
        }
        [HttpPost]
        public ActionResult EditItems(Items item)
        {
            var items = DBContext.Items.Where(e => e.Id == item.Id).FirstOrDefault();
            items.Name = item.Name;
            items.Remark = item.Remark;
            var delObj = DBContext.ItemsResources.Where(e => e.ItemId == items.Id);
            foreach (var v in delObj)
                DBContext.DeleteObject(v);
            if (Request.Form["resourceid"] != null && Request.Form["diveceid"] != null)
            {
                string[] resources = Request.Form["resourceid"].TrimEnd(',').Split(',');
                string[] devices = Request.Form["diveceid"].TrimEnd(',').Split(',');
                string str = Request.Form["PlayName"];
                for (int i = 0; i < resources.Length; i++)
                {
                    if (resources[i] != string.Empty && devices[i] != string.Empty)
                    {
                        ItemsResources ir = new ItemsResources { ItemId = item.Id, ResourceId = int.Parse(resources[i]), DeviceId = int.Parse(devices[i]) };
                        DBContext.AddToItemsResources(ir);
                    }
                }
            }
            DBContext.SaveChanges();
            return RedirectToAction("IndexItems");
        }
        public ActionResult IndexItems()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            var list = DBContext.Items.ToList();
            return View(list);
        }
        public ActionResult DetailsItems(int ID)
        {
            var item = DBContext.Items.Where(e => e.Id == ID).FirstOrDefault();
            return View(item);
        }
        //public ActionResult AddPlayItems(int ID)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    var resources = DBContext.Resources.Select(e => e.Category).Distinct().ToList();
        //    foreach (var v in resources)
        //        list.Add(new SelectListItem { Text = v, Value = v });
        //    ViewData["ResourcesType"] = list;
        //    List<SelectListItem> list1 = new List<SelectListItem>();
        //    var devices = DBContext.Device.ToList();
        //    foreach (var v in devices)
        //    {
        //        list1.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });
        //    }
        //    ViewData["Devices"] = list1;
        //    var item = DBContext.Items.FirstOrDefault(e => e.Id == ID);
        //    return View(item);
        //}
        //public ActionResult EditPlayItems(int ID)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    var resources = DBContext.Resources.Select(e => e.Category).Distinct().ToList();
        //    foreach (var v in resources)
        //        list.Add(new SelectListItem { Text = v, Value = v });
        //    ViewData["ResourcesType"] = list;
        //    List<SelectListItem> list1 = new List<SelectListItem>();
        //    var devices = DBContext.Device.ToList();
        //    foreach (var v in devices)
        //    {
        //        list1.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });
        //    }
        //    ViewData["Devices"] = list1;
        //    var isobj = DBContext.ItemsResources.FirstOrDefault(e => e.Id == ID);
        //    var item = DBContext.Items.FirstOrDefault(e => e.Id == isobj.ItemId);
        //    ViewData["PlayName"] = isobj.PlayName;
        //    return View(item);
        //}

        public JsonResult InitPlayResource(int? ID)
        {
            if (ID != null)
            {
                var objlist = DBContext.ItemsResources.Where(c => c.ItemId == ID);
                List<string> list = new List<string>();
                foreach (var v in objlist)
                {
                    string result = DBContext.Resources.FirstOrDefault(e => e.Id == v.ResourceId).Name+",";
                    result += DBContext.Device.FirstOrDefault(e => e.Id == v.DeviceId).Name + ",";
                    result += v.ResourceId.ToString() + "," + v.DeviceId;
                    list.Add(result);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        public void DeletePlayResource(int? ID, string Name)
        {
            if (ID != null)
            {
                var list = DBContext.ItemsResources.Where(e => e.ItemId == ID);
                foreach (var v in list)
                {
                    DBContext.DeleteObject(v);
                }
                DBContext.SaveChanges();
            }
        }

        #endregion

        #region Package
        public ActionResult IndexItemsPackage()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            var list = DBContext.ItemsPackage.ToList();
            return View(list);
        }
        public ActionResult AddItemsPackage()
        {
            ViewData["Items"] = DBContext.ShowItemsList.ToList().Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Name }).ToList<SelectListItem>();
            return View();
        }
        [HttpPost]
        public ActionResult AddItemsPackage(ItemsPackage package)
        {
            package.CreateTime = DateTime.Now;
            DBContext.ItemsPackage.AddObject(package);
            DBContext.SaveChanges();
            if (Request.Form["itemid"] != null && Request.Form["listid"] != null)
            {
                string[] items = Request.Form["itemid"].Split(',');
                string[] listids = Request.Form["listid"].Split(',');

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != string.Empty && listids[i] != string.Empty)
                    {
                        PackageItems ir = new PackageItems { ItemId = int.Parse(items[i]), PackageId = package.Id, ListId = int.Parse(listids[i]) };
                        DBContext.AddToPackageItems(ir);
                    }
                }
                DBContext.SaveChanges();
            }
            return RedirectToAction("IndexItemsPackage");
        }
        public ActionResult EditItemsPackage(int ID)
        {
            ViewData["Items"] = DBContext.ShowItemsList.ToList().Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Name }).ToList<SelectListItem>();
            var item = DBContext.ItemsPackage.FirstOrDefault(e => e.Id == ID);
            return View(item);
        }
        [HttpPost]
        public ActionResult EditItemsPackage(ItemsPackage package)
        {
            var obj = DBContext.ItemsPackage.FirstOrDefault(e => e.Id == package.Id);
            obj.Name = package.Name;
            obj.Remark = package.Remark;
            var list = DBContext.PackageItems.Where(e => e.PackageId == package.Id);
            foreach (var v in list)
                DBContext.DeleteObject(v);
            if (Request.Form["itemid"] != null && Request.Form["listid"] != null)
            {
                string[] items = Request.Form["itemid"].Split(',');
                string[] listids = Request.Form["listid"].Split(',');

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != string.Empty && listids[i] != string.Empty)
                    {
                        PackageItems ir = new PackageItems { ItemId = int.Parse(items[i]), PackageId = package.Id, ListId = int.Parse(listids[i]) };
                        DBContext.AddToPackageItems(ir);
                    }
                }
            }
            DBContext.SaveChanges();
            return RedirectToAction("IndexItemsPackage");
        }
        public JsonResult InitPackage(int? ID)
        {
            if (ID != null)
            {
                var objlist = DBContext.PackageItems.Where(c => c.PackageId == ID);
                List<string> list = new List<string>();
                foreach (var v in objlist)
                {
                    string result = DBContext.ShowItemsList.FirstOrDefault(e => e.Id == v.ListId).Name + "," + DBContext.Items.FirstOrDefault(e => e.Id == v.ItemId).Name + ",";
                    result += v.ListId.ToString() + "," + v.ItemId.ToString();
                    list.Add(result);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        
        public JsonResult GetPlay(int? ID)
        {
            if (ID.HasValue)
            {
                var list = DBContext.Items.Where(e => e.ListId == ID).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        public void DeletePackage(string ID)
        {
            List<int> result = new List<string>(ID.TrimEnd(',').Split(',')).ConvertAll(i => int.Parse(i));
            foreach (int i in result)
            {
                DBContext.DeleteObject(DBContext.ItemsPackage.FirstOrDefault(e => e.Id == i));
            }
            DBContext.SaveChanges();
        }
        #endregion

        #region Natures
        //public ActionResult Calendar()
        //{
        //    var list = DBContext.Natures.ToList();
        //    return PartialView(list);
        //}
        public ActionResult Calendar(string id)
        {
            var list = DBContext.Natures.ToList();
            if (id != null)
                ViewData["date"] = id;
            return PartialView(list);
        }
        public ActionResult AddNatures(string id)
        {
            ViewData["date"] = id;
            //List<int> result = new List<string>(id.Split('-')).ConvertAll(i => int.Parse(i));
            //int year = result[0];
            //int mon = result[1];
            //int day = result[2];
            //int[] time = { 9, 10, 11, 14, 15, 16 };
            //string timeradio = "";
            //var list = DBContext.Natures.Where(e => e.Date.Year == year && e.Date.Month == mon && e.Date.Day == day);
            //foreach (int i in time)
            //{
            //    string str = "";
            //    if (list.Where(e => e.Time == i).ToList().Count > 0)
            //        str = "disabled='disabled'";
            //    timeradio += @"<label class='checkbox'><input type='radio' name='time' value='" + i.ToString() + "' " + str + "/>" + i.ToString().PadLeft(2, '0') + ":00</label>";
            //}
            //ViewData["times"] = timeradio;
            return View();
        }
        [HttpPost]
        public ActionResult AddNatures(Natures na)
        {
            na.Status = "等待";
            na.Feedback = "";
            DBContext.AddToNatures(na);
            DBContext.SaveChanges();
            return RedirectToAction("Calendar");
        }
        public ActionResult DetailNatures(int ID)
        {
            var obj = DBContext.Natures.FirstOrDefault(e => e.Id == ID);
            var list = DBContext.NaturesItemsSet.Where(e => e.Id == ID).Select(e=>e.ListId);
            var slist = DBContext.ShowItemsList.Where(e => list.Contains(e.Id) == true);
            List<SelectListItem> select = new List<SelectListItem>();
            foreach (var v in slist)
            {
                select.Add(new SelectListItem { Value = v.Id.ToString(), Text = v.Name });
            }
            ViewData["list"] = select;
            return View(obj);
        }
        [HttpPost]
        public ActionResult DetailNatures(Natures na)
        {
            var obj = DBContext.Natures.FirstOrDefault(e => e.Id == na.Id);
            obj.Feedback = na.Feedback == null ? "" : na.Feedback;
            if (obj.Feedback != "")
                obj.Status = "已参观";
            var list = DBContext.NaturesItemsSet.Where(e => e.NaId == na.Id);
            foreach (var v in list)
            {
                DBContext.DeleteObject(v);
            }
            if (Request.Form["resourceid"] != null && Request.Form["playname"] != null)
            {
                string[] items = Request.Form["resourceid"].Split(',');
                string[] playnames = Request.Form["playname"].Split(',');

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != string.Empty && playnames[i] != string.Empty)
                    {
                        NaturesItems ir = new NaturesItems { ListId = int.Parse(items[i]), ItemId = int.Parse(playnames[i]), NaId = na.Id };
                        DBContext.AddToNaturesItemsSet(ir);
                    }
                }
            }
            DBContext.SaveChanges();
            var follow = DBContext.NaturesItemsSet.Where(e => e.NaId == na.Id).ToList();
            foreach (var v in follow)
            {
                v.FollowLevel = "";
            }
            if (Request.Form["follow"] != null && Request.Form["level"] != null)
            {
                string[] listids = Request.Form["resourceid"].Split(',');
                string[] levels = Request.Form["level"].Split(',');
                for (int i = 0; i < listids.Length; i++)
                {
                    int id = int.Parse(listids[i]);
                    var result = follow.Where(e => e.ListId == id && e.NaId == na.Id);
                    foreach (var r in result)
                    {
                        r.FollowLevel = levels[i];
                    }
                }
            }
            DBContext.SaveChanges();
            return RedirectToAction("Calendar");
        }
        public JsonResult DeleteNature(int ID)
        {
            var obj = DBContext.Natures.FirstOrDefault(e => e.Id == ID);
            obj.Status = "取消预约";
            DBContext.SaveChanges();
            return null;
        }
        public JsonResult GetItems()
        {
            var list = DBContext.ShowItemsList.ToList();
            return Json(list, JsonRequestBehavior.AllowGet); ;
        }
        public JsonResult GetPlayItems(int ID, int name)
        {
            if (name == 1)
            {
                var q = from s in DBContext.ShowItemsList
                        join p in DBContext.PackageItems on s.Id equals p.ListId
                        where p.PackageId == ID
                        select s;
                return Json(q.ToList(), JsonRequestBehavior.AllowGet);
            }
            if (name == 2)
                return Json(DBContext.Items.Where(e => e.ListId == ID).ToList(), JsonRequestBehavior.AllowGet);
            return null;
        }
        //public JsonResult GetPackageItems(int ID)
        //{
        //    if (name == 1)
        //    {
        //        var q = from s in DBContext.ShowItemsList
        //                join p in DBContext.PackageItems on s.Id equals p.ListId
        //                where p.PackageId == ID
        //                select s;
        //        return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        //    }
        //    if (name == 2)
        //        return Json(DBContext.Items.Where(e => e.ListId == ID).ToList(), JsonRequestBehavior.AllowGet);
        //    return null;
        //}
        public JsonResult GetPackages()
        {
            return Json(DBContext.ItemsPackage.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsPackages(int ID)
        {
            var q = from p in DBContext.PackageItems
                    join s in DBContext.ShowItemsList on p.ListId equals s.Id
                    join i in DBContext.Items on p.ItemId equals i.Id
                    where p.PackageId == ID
                    select new { listname = s.Name, itemname = i.Name, listid = s.Id, itemid = i.Id };
            return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListItems(int ID)
        {
            var q = from p in DBContext.PackageItems
                    join s in DBContext.ShowItemsList on p.ListId equals s.Id
                    join i in DBContext.Items on p.ItemId equals i.Id
                    where p.PackageId == ID
                    select s;
            return Json(DBContext.Items.Where(e => e.ListId == ID).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStatus(int ID,string name)
        {
            var obj = DBContext.Natures.FirstOrDefault(e => e.Id == ID);
            obj.Status = name;
            DBContext.SaveChanges();
            return null;
        }
        public JsonResult InitNaturesPackage(int? ID)
        {
            if (ID != null)
            {
                var objlist = DBContext.NaturesItemsSet.Where(c => c.NaId == ID);
                List<string> list = new List<string>();
                foreach (var v in objlist)
                {
                    string result = DBContext.ShowItemsList.FirstOrDefault(e => e.Id == v.ListId).Name + "," + DBContext.Items.FirstOrDefault(e => e.Id == v.ItemId).Name + ",";
                    result += v.ListId.ToString() + "," + v.ItemId;
                    list.Add(result);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        public JsonResult InitFollowLevel(int? ID)
        {
            if (ID != null)
            {
                var q = from p in DBContext.NaturesItemsSet
                        where p.NaId == ID && p.FollowLevel != ""
                        group p by new
                        {
                            p.ListId,
                            p.FollowLevel
                        }
                            into g
                            select new
                            {
                                g.Key,
                                g
                            };
                List<string> list = new List<string>();
                foreach (var v in q)
                {
                    string result = DBContext.ShowItemsList.FirstOrDefault(e => e.Id == v.Key.ListId).Name + "," + v.Key.ListId + ",";
                    result += v.Key.FollowLevel;
                    list.Add(result);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }
        #endregion

        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(Users user)
        {
            DBContext.AddToUsers(user);
            DBContext.SaveChanges();
            AuthorizationToUser(user.Id, user.RoleId);
            return RedirectToAction("IndexUser");
        }
        public ActionResult EditUser(int ID)
        {
            return View(DBContext.Users.FirstOrDefault(e => e.Id == ID));
        }
        [HttpPost]
        public ActionResult EditUser(Users user)
        {
            var obj = DBContext.Users.FirstOrDefault(e => e.Id == user.Id);
            if (user.Password != null && user.Password.Trim() != string.Empty)
                obj.Password = user.Password;
            obj.Number = user.Number;
            obj.Name = user.Name;
            obj.Tel = user.Tel;
            obj.Email = user.Email;
            obj.Remark = user.Remark;
            if (obj.RoleId != user.RoleId)
            {
                var result = DBContext.UsersRights.Where(e => e.UserId == obj.Id);
                foreach (var v in result)
                    DBContext.DeleteObject(v);
                obj.RoleId = user.RoleId;
                AuthorizationToUser(obj.Id, user.RoleId);
            }
            else
            {
                DBContext.SaveChanges();
            }
            return RedirectToAction("IndexUser");
        }
        public JsonResult DeleteUser(string ID)
        {
            List<int> result = new List<string>(ID.TrimEnd(',').Split(',')).ConvertAll(i => int.Parse(i));
            var list = DBContext.Users.Where(e => result.Contains(e.Id) == true);
            foreach (var v in list)
            {
                DBContext.DeleteObject(v);
            }
            var rights = DBContext.UsersRights.Where(e => result.Contains(e.UserId) == true);
            foreach (var v in rights)
            {
                DBContext.DeleteObject(v);
            }
            DBContext.SaveChanges();
            return null;
        }
        public ActionResult IndexUser()
        {
            ViewData["pageInput"] = true;
            ViewData["nextPrevious"] = true;
            ViewData["numeric"] = true;
            var list = DBContext.Users.ToList();
            return View(list);
        }
        public ActionResult UserPassword()
        {
            int id = int.Parse(this.User.Identity.Name);
            return View(DBContext.Users.FirstOrDefault(e => e.Id == id));
        }
        public JsonResult ChangedPassword(string cpwd,string npwd)
        {
            try
            {
                int id = int.Parse(this.User.Identity.Name);
                var obj = DBContext.Users.FirstOrDefault(e => e.Id == id);
                if (cpwd == obj.Password)
                {
                    obj.Password = npwd;
                    DBContext.SaveChanges();
                    new FormsAuthenticationService().SignOut();
                    return this.Json(new
                    {
                        ResultBool = true
                    });
                }
                else
                {
                    return this.Json(new
                    {
                        ResultBool = false
                    });
                }
            }
            catch
            {
                return this.Json(new
                {
                    ResultBool = false
                });
            }
            
        }
        public JsonResult CheckPassword(string ID)
        {
            int id = int.Parse(this.User.Identity.Name);
            var obj = DBContext.Users.FirstOrDefault(e => e.Id == id);
            if (ID == obj.Password)
            {
                return this.Json(new
                {
                    ResultBool = true
                });
            }
            else
            {
                return this.Json(new
                {
                    ResultBool = false
                });
            }
        }
        public void AuthorizationToUser(int userId,int RoleId)
        {
            var rights = DBContext.RolesRights.Where(e => e.RoleId == RoleId).ToList();
            bool sumbit = false;
            for (int i = 0; i < rights.Count; i++)
            {
                if (i == rights.Count - 1)
                    sumbit = true;
                AuthorizationToUser(userId, rights[i].RightId, sumbit);
            }
        }
        public void AuthorizationToUser(int userId, int rightId, bool isSumbit)
        {
            DBContext.AddToUsersRights(new UsersRights { UserId = userId, RightId = rightId });
            if (isSumbit)
                DBContext.SaveChanges();
        }

        [HttpPost]
        public JsonResult OptionPlay(int ID,string name)
        {
            try
            {
                InstanceContext ctx = new InstanceContext(this);
                ControlService.MainControlClient svc = new ControlService.MainControlClient(ctx);
                svc.ClientCredentials.Windows.ClientCredential.UserName = System.Configuration.ConfigurationManager.AppSettings["user"];
                svc.ClientCredentials.Windows.ClientCredential.Password = System.Configuration.ConfigurationManager.AppSettings["pwd"];
                svc.Option2((ControlService.OptionType)ID, name.Replace("-", ":"));
                svc.ChannelFactory.Close();
            }
            catch {
                return Json("无法联系服务", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult StartPlay(string id, string name)
        {
            try
            {
                InstanceContext ctx = new InstanceContext(this);
                ControlService.MainControlClient svc = new ControlService.MainControlClient(ctx);
                svc.ClientCredentials.Windows.ClientCredential.UserName = System.Configuration.ConfigurationManager.AppSettings["user"];
                svc.ClientCredentials.Windows.ClientCredential.Password = System.Configuration.ConfigurationManager.AppSettings["pwd"];
                //svc.Option2(ControlService.OptionType.Start, ShowRoomSys.CommonLib.Common.MAC);
                svc.Option3(ControlService.OptionType.Start, id.Replace("-", ":"), name);
                svc.ChannelFactory.Close();
            }
            catch {
                return Json("无法联系服务", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void SendMessage(ControlService.OptionType type)
        {
            //MessageBox.Show(string.Format("[ClientTime{0:HHmmss}]Service Broadcast:{1}", DateTime.Now, message));
        }
        public void SendMessages(ControlService.OptionType type, string fileName)
        {
            throw new NotImplementedException();
        }

        #region upload & save file
        [HttpPost]
        public ContentResult UploadFile(HttpPostedFileBase FileData, string folder)
        {
            string result = "";
            if (null != FileData)
            {
                try
                {
                    result = Path.GetFileName(FileData.FileName); 
                    string saveName = result; 
                    result = SaveFile(FileData, folder, saveName); 
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return Content(result);
        }
        [NonAction]
        private string SaveFile(HttpPostedFileBase postedFile, string filepath, string saveName)
        {
            string phyPath = Request.MapPath("~" + filepath + "/");
            if (!Directory.Exists(phyPath))
            {
                Directory.CreateDirectory(phyPath);
            }
            try
            {
                postedFile.SaveAs(phyPath + saveName);
                string ext = Path.GetExtension(phyPath + saveName).ToLower();
                if (ext == ".doc" || ext == ".docx" || ext == ".ppt" || ext == ".pptx" || ext == ".xls" || ext == ".xlsx")
                {
                    switch (ConvertToXps(phyPath, saveName))
                    {
                        case ConversionResult.OK:
                            return saveName;
                        case ConversionResult.InvalidFilePath:
                            {
                                //throw new Exception("文件不存在");
                                return "文件不存在";
                            }
                        case ConversionResult.UnexpectedError:
                            {
                                //throw new Exception("未知错误");
                                return "未知错误";
                            }
                        case ConversionResult.ErrorUnableToOpenOfficeFile:
                            {
                                //throw new Exception("文件被占用");
                                return "文件被占用";
                            }
                        case ConversionResult.ErrorUnableToInitializeOfficeApp:
                        case ConversionResult.ErrorUnableToAccessOfficeInterop:
                        case ConversionResult.ErrorUnableToExportToXps:
                            {
                                //throw new Exception("请安装OFFICE 2007或以上版本");
                                return "请安装OFFICE 2007或以上版本";
                            }
                    }
                }
                else
                    return saveName;
                return "";
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        private ConversionResult ConvertToXps(string phyPath,string fileName)
        {
            string xpsFilePath = phyPath + Path.GetFileNameWithoutExtension(fileName) + ".xps";
            return OfficeToXps.ConvertToXps(phyPath + fileName, ref xpsFilePath).Result;
        }
        #endregion



    }
}
