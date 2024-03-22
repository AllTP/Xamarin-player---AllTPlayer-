using AllTPlayer_App.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace AllTPlayer_App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}