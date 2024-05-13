using EsCustomer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EsCustomer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Viewpage : ContentPage
    {
        private Brands brands;
        public Brands Brands
        {
            get
            {
                return this.brands;
            }
            set
            {
                this.brands = value;
                OnPropertyChanged(nameof(Brands));
            }
        }
        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new Command(async () =>
                    {
                        this.IsRefreshing = true;

                        await this.RefreshBrandList();

                        this.IsRefreshing = false;
                    });
                }
                return refreshCommand;
            }
        }

        

        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get
            {
                return this.isRefreshing;
            }
            set
            {
                this.isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public string SearchTerm { get; set; }

        public Viewpage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        private async Task RefreshBrandList()
        {

            Expression<Func<Brands, bool>> searchLambda = x => x.BrandName.Contains("SearchTerm");

            // Convert the lambda to a string and apply a string replace operation to replace the "SearchTerm" string with the actual value
            var stringLambda = searchLambda.ToString().Replace("SearchTerm", $"{SearchTerm}");

            // Parse the "hacked" string to reform the Lambda expression again using ParseLambda
            searchLambda = DynamicExpressionParser.ParseLambda<Brands, bool>(new ParsingConfig(), true, stringLambda, this.SearchTerm);

            // Pass the prepared lambda to the DataService call as normal 
            listView.ItemsSource = await App.DataService.GetAllAsync<Brands>(searchLambda, "GetBrandsWithRelatedData");

            
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.BeginRefresh();
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                
                {
                    await Navigation.PushAsync(new Viewpage
                    {
                        Brands = e.SelectedItem as Brands
                    });
                }
            }
        }


        private void SearchTerm_Completed(object sender, EventArgs e)
        {
            this.RefreshCommand.Execute(null);
        }
    }
}