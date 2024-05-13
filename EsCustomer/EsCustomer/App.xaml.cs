using EsCustomer.Data;
using EsCustomer.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EsCustomer
{
    public partial class App : Application
    {
        static DataService dataService;

        public static DataService DataService
        {
            get
            {
                if (dataService == null)
                {
                    dataService =
                        new DataService("https://192.168.137.1:5001")
                            .AddEntityModelEndpoint<Brands>("api/Brands")
                            .AddEntityModelEndpoint<ImageFile>("api/ImageFiles");


                }
                return dataService;
            }

        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }
    }
    
}
