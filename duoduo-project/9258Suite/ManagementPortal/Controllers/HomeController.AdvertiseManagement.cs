using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YoYoStudio.DataService.Client.Generated;
using YoYoStudio.Model.Media;
using YoYoStudio.Model.Core;
using YoYoStudio.Model;
using YoYoStudio.Common;
using YoYoStudio.Exceptions;
using YoYoStudio.Model.Json;
using System.Windows.Forms;

namespace YoYoStudio.ManagementPortal.Controllers
{
    public partial class HomeController
    {
        public ActionResult AdvertiseManagement()
        {
           return PartialView("PartialViews/AdvertiseManagement", new YoYoStudio.Model.Json.JsonModel());
        }

        public JsonResult GetImageTypes()
        {
            List<JsonModel> result = new List<JsonModel>();
            foreach (var type in BuiltIns.ImageTypes)
            {
                if (type.Id <= 16 && type.Id >= 12)
                {
                    JsonModel jm = new JsonModel();
                    jm.Id = type.Id;
                    jm.Name = type.Name;
                    jm.Description = type.Description;
                    result.Add(jm);
                }
            }
            return Json(new {Success = true, Rows = result.ToArray() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdvertise(string page, string pageSize)
        {
            string message = string.Empty;
            bool success = false;
            List<ImageModel> result = new List<ImageModel>();
            int total = 0;
            try
            {
                string condition = string.Empty;
                condition = "([ImageType_Id] = 12 OR [ImageType_Id] = 13 OR [ImageType_Id] = 14 OR [ImageType_Id] = 15 OR [ImageType_Id] = 16)";
                var allImgs = GetEntities<ImageWithoutBody>(page, pageSize, out total, condition);
                foreach(var img in allImgs)
                {
                    ImageWithoutBody imwb = img as ImageWithoutBody;
                    ImageModel im = new ImageModel();
                    im.Name = imwb.Name;
                    im.Link = imwb.Link;
                    im.ImageType_Id = imwb.ImageType_Id;
                    im.Id = imwb.Id;
                    im.Ext = imwb.Ext;
                    result.Add(im);
                }
                success = true;
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }

            return Json(new {Success = success,Message = message, Rows = result.ToArray(), Total = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAdvertise(List<ImageModel> ads)
        {
            return SaveEntities<Image>(ads);
        }

        [HttpPost]
        public JsonResult NewAdvertise()
        {
            try
            {
                Image img = AddEntity<Image>(new Image { Ext = ".png", Link = "http://www.9258.tv", ImageType_Id = BuiltIns.GlobalCompanyAdvPics.Id, Name = "默认", 
                    TheImage = Utility.GetImageBytes(Properties.Resources.NotFound)});
                return Json(new { Success = true, Model = new ImageModel(string.Empty,img), Message = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException exception)
            {
                return Json(new { Success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
