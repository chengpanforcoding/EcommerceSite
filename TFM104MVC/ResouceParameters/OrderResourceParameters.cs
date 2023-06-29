using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.ResouceParameters
{
    public class OrderResourceParameters
    {
        public string Status { get; set; }
        public string Keyword { get; set; }
        
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value >= 1)
                {
                    _pageSize= (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if(value >= 1)
                {
                    _pageNumber = value;
                }
            }
        }

        private int _pageNumber = 1;
        private int _pageSize = 10;
        const int maxPageSize = 50;
    }
}
