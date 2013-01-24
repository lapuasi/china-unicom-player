using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShowRommSys.Client.Web.Models;

namespace ShowRommSys.Client.Web.Common
{
    public class FunctionMenu
    {
        public static List<MenuItem> GetFunctionMenus(int userid)
        {
            using (ShowRoomSysEntities bse = new ShowRoomSysEntities())
            {
                List<MenuItem> results = new List<MenuItem>();
                List<MenuItems> First = bse.MenuItem.Where(p => p.PId == 0 && p.Visible == true).OrderBy(f => f.Index).ToList();
                List<string> list;
                var q = from u in bse.UsersRights
                        join r in bse.Rights on u.RightId equals r.Id
                        where u.UserId == userid
                        select r.Code;
                list = q.ToList();
                var i = from item in bse.MenuItem
                            where item.PId != 0 && item.Visible == true && list.Contains(item.URL)==true
                            orderby item.Index
                            select new MenuItem { icon = item.IconStyle, menuid = item.Id, menuname = item.Name, url = item.URL,pid=item.PId};
                List<MenuItem> items = i.ToList();
                for(int m=0;m<First.Count;m++)
                {
                    var sublist = items.Where(e => e.pid == First[m].Id).ToList();
                    if (sublist.Count > 0)
                    {
                        results.Add(new MenuItem { menuid = First[m].Id, menuname = First[m].Name, menus = sublist });
                    }
                }
                return results;
            }
        }
    }
}