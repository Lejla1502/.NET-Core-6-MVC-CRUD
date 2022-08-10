﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Pagination
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager()
        {

        }

        public Pager(int totalItems, int page, int pageSize = 5)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 4;
            int endPage = currentPage + 3;

            if(startPage<=0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if(endPage>totalPages)
            {
                endPage = totalPages;
                if (endPage > 5)
                    startPage = endPage - 4;
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;

            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }
    }

    
}
