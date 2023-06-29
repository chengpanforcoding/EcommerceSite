using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models;
using TFM104MVC.Models.Enum;

namespace TFM104MVC.ResouceParameters
{
    public class ProductResourceParameters
    {
        public string OrderByDesc { get; set; }
        public string OrderBy { get; set; }
        public string Keyword { get; set; }
        public string Rating { get; set; }
        public string Region { get; set; }
        public string Traveldays { get; set; }
        public string Triptype { get; set; }
        public string GoTouristTime { get; set; }
        public string ProductStatus { get; set; }
        public int PageNumber { 
            get 
            { 
                return _pageNumber; 
            } 
            set 
            {
                if (value >= 1) 
                {
                    _pageNumber = value;
                }
            } 
        }
        public int PageSize {
            get 
            {
                return _pageSize;
            }
            set 
            {
                if (value >= 1)
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            } 
        }
        private int _pageNumber = 1;
        private int _pageSize = 10;
        const int maxPageSize = 50;
        public List<string> Regions { get; set; } = new();
    }
}
