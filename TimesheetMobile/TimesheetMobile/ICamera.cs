using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetMobile
{
    public interface ICamera
    {
        void TakePicture(string employeeName);

        Action PictureTaken { get; set; }
    }
}
