using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EsCustomer.Models
{
    public class Brands :   INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }

        public string BrandName { get; set; }

        public int EthicalScore { get; set; }

        public string Link1 { get; set; }

        public string Link2 { get; set; }





        private string productReference; // backing field
        public string ProductReference // property
        {
            get
            {
                return this.productReference;
            }
            set
            {
                this.productReference = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductReference)));
            }
        }


        [JsonIgnore]
        public string DetailsSummary
        {
            get
            {
                var summaryStrings = new List<string>();
                summaryStrings.Add(this.Brands.ToString());
                if (this.Brands != null)
                {
                    summaryStrings.Add(this.Brands?.BrandName);
                }
                return string.Join(", ", summaryStrings);
            }
        }



        public int? ImageFileId { get; set; }

        private ImageFile imageFile;
        public ImageFile ImageFile
        {
            get
            {
                return this.imageFile;
            }
            set
            {
                this.imageFile = value;
                if (this.ImageFile != null)
                {
                    this.ImageFileId = this.ImageFile.Id;
                }
                else
                {
                    this.ImageFileId = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageFile)));
            }
        }

    }
}
