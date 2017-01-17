using CYINT.XPlatformNetworking.AndroidImplementation;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using CYINT.XPlatformNetworking;
using Android.Net;
using Android.Content;
using Xamarin.Forms;
using PCLStorage;

[assembly: Xamarin.Forms.Dependency (typeof (AndroidNetworkingImplementation))]
namespace CYINT.XPlatformNetworking.AndroidImplementation
{
    public class AndroidNetworkingImplementation : IXPlatformNetworking
    {

        protected Context _appContext;
        protected ConnectivityManager _connectivityManager;
        public const string cacheFolderName = "AppCache";

        public AndroidNetworkingImplementation()
        {       
            SetAppContext(Android.App.Application.Context);        
        }

        public bool IsNetworkAvailable()
        {         
            NetworkInfo activeConnection = GetConnectivityManager().ActiveNetworkInfo;
            return (activeConnection != null) && activeConnection.IsConnected;
        }

        public void SetAppContext(Context appContext)
        {
            _appContext = appContext;
        }

        public Context GetAppContext()
        {
            return _appContext;
        }

        public void SetConnectivityManager(ConnectivityManager connectivityManager)
        {
            _connectivityManager = connectivityManager;
        }

        public ConnectivityManager GetConnectivityManager()
        {
            if(_connectivityManager == null && GetAppContext() != null)
            {
                SetConnectivityManager((ConnectivityManager) GetAppContext().GetSystemService(Context.ConnectivityService));
            }
            else if (GetAppContext() == null || _connectivityManager == null)
                throw new AndroidNetworkingImplementationException("Could not get connectivity manager. Context not found.");

            return _connectivityManager;
        }

        public async void PlaceInLocalStorage(string key, string content)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;            
            IFolder cacheFolder = await rootFolder.CreateFolderAsync(cacheFolderName, CreationCollisionOption.OpenIfExists);         
            IFile file = await cacheFolder.CreateFileAsync(key + ".cache", CreationCollisionOption.ReplaceExisting);                      
            await file.WriteAllTextAsync(content);
        }

        public async Task<string> RetrieveFromLocalStorage(string key)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;   
            IFolder cacheFolder = await rootFolder.CreateFolderAsync(cacheFolderName, CreationCollisionOption.OpenIfExists);          
            IFile file = await cacheFolder.GetFileAsync(key + ".cache");      
            string content = await file.ReadAllTextAsync();        
            return content;
        }

    }

    public class AndroidNetworkingImplementationException : Exception
    {
        public AndroidNetworkingImplementationException(string message) : base(message) { }
    }
}