using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ML
{
    public  class ProductRating
    {
        [LoadColumn(0)]
        public string userId;

        [LoadColumn(1)]
        public float productId;

        [LoadColumn(2)]
        public float Label;
    }
}
