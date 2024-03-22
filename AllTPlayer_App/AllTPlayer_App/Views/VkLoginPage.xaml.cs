using Microsoft.Extensions.DependencyInjection;
using AllTPlayer_App.Models;
using AllTPlayer_App.Views;
using AllTPlayer_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VkNet.AudioBypassService.Exceptions;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace AllTPlayer_App
{
    public partial class VkLoginPage : ContentPage
    {
        public VkLoginPage()
        {
            InitializeComponent();
        }

        private async void LoginSubmitButton_Clicked(object sender, EventArgs e)
        {
            var api = new VkApi(new ServiceCollection().AddAudioBypass());
            try
            {
                errorText.IsVisible = false;
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 51608982,
                    Login = login.Text,
                    Password = password.Text,
                    Settings = VkNet.Enums.Filters.Settings.All,
                });

                await App.UsersDB.SaveUserAsync(new Models.User { Id = 0, Login = login.Text, Password = password.Text });

                await Shell.Current.Navigation.PushAsync(new VkMusicPage(), true);
            }
            catch (VkNet.AudioBypassService.Exceptions.VkAuthException)
            {
                errorText.IsVisible = true;
            }
            catch (VkNet.Exception.CaptchaNeededException)
            {
                errorText.Text = "Слишком много попыток, попробуйте через минуту";
                errorText.IsVisible = true;
            }

            
        }
    }
}