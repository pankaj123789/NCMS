using System.Collections.Generic;
using System.Linq;
using MyNaati.Ui.ViewModels.Products;

namespace MyNaati.Ui.ViewModels.IdCard
{
    public class SelectOrderEditModel
    {
        private List<AvailableProduct> mAccreditations;
        private List<AvailableProduct> mRecognitions;

        public int OrderOption
        {
            get
            {
                int orderOption = 0;

                if (IsOrderingSingle)
                {
                    orderOption = 1;
                }
                else if (IsOrderingForAccreditations && IsOrderingForRecognitions)
                {
                    orderOption = 2;
                }
                else if (IsOrderingForAccreditations)
                {
                    orderOption = 3;
                }
                else if (IsOrderingForRecognitions)
                {
                    orderOption = 4;
                }

                return orderOption;
            }
            set
            {
                IsOrderingSingle = (value == 1);
                IsOrderingForAccreditations = (value == 2 || value == 3);
                IsOrderingForRecognitions = (value == 2 || value == 4);
            }
        }

        public bool IsOrderingSingle { get; set; }
        public bool IsOrderingForAccreditations { get; set; }
        public bool IsOrderingForRecognitions { get; set; }

        public List<AvailableProduct> Accreditations
        {
            get 
            { 
                return mAccreditations; 
            }
            set
            {
                mAccreditations = value.OrderBy(e => e.Skill)
                                       .ThenBy(e => e.Level)
                                       .ThenBy(e => e.Expiry)
                                       .ToList();
            }
        }

        public List<AvailableProduct> Recognitions
        {
            get 
            { 
                return mRecognitions; 
            }
            set
            {
                mRecognitions = value.OrderBy(e => e.Skill)
                                       .ThenBy(e => e.Level)
                                       .ThenBy(e => e.Expiry)
                                       .ToList();
            }
        }
    }
}