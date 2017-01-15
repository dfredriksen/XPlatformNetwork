using CYINT.XPlatformNetworking.AndroidImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using CYINT.XPlatformNetworking;
using Android.Net;
using Android.Content;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency (typeof (AndroidNetworkingImplementation))]
namespace CYINT.XPlatformNetworking.AndroidImplementation
{
    public class AndroidNetworkingImplementation : IXPlatformNetworking
    {

        protected Context _appContext;
        protected ConnectivityManager _connectivityManager;

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
            else
                throw new AndroidNetworkingImplementationException("Could not get connectivity manager. Context not found.");

            return _connectivityManager;
        }
    }

    public class AndroidNetworkingImplementationException : Exception
    {
        public AndroidNetworkingImplementationException(string message) : base(message) { }
    }
}