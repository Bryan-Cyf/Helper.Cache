using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruanmou.BLL
{
    /// <summary>
    /// 读硬盘文件
    /// </summary>
    public class FileHelper
    {
        public static List<T> Query<T>(int index)
        {
            Console.WriteLine("This is {0} Query", typeof(FileHelper));
            long lResult = 0;
            for (int i = index; i < 1000000000; i++)
            {
                lResult += i;
            }

            return new List<T>()
            {
                default(T),default(T),default(T)
            };
        }

    }
}
