using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Travel_Agency___Data.ViewModels
{
    public class ProfilePictureViewModel
    {
        public int CustomerId { get; set; }
        public string CurrentPictureUrl { get; set; }
        public Stream NewPicture { get; set; }
    }
}
