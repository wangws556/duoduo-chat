using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YoYoStudio.Model.Core;

namespace YoYoStudio.Model.Json
{  
    public class ImageModel : JsonModel
    {
        public string Ext { get; set; }
        public string OwnerType { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public byte[] TheImage { get; set; }
        public int ImageType_Id { get; set; }

        public ImageModel() : this(string.Empty) { }
        public ImageModel(string ownertype)
            : this(ownertype,null)
        {
        }
        public ImageModel(string ownertype, Image image)
            : base(image)
        {
            if (image != null)
            {
                Name = image.Name;
                Ext = image.Ext;
                Link = image.Link;
                TheImage = image.TheImage;
                ImageType_Id = image.ImageType_Id;
            }
            OwnerType = ownertype;
        }

        protected override YoYoStudio.Model.ModelEntity CreateModelEntity()
        {
            return new Image { Name = Name, Ext = Ext, Link = Link, ImageType_Id = ImageType_Id,
            TheImage = TheImage};
        }

        public int OwnerId { get; set; }

    }
}