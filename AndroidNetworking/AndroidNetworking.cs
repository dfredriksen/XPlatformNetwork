using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using CYINT.XPlatformNetworking;
using Android.Net;
using Android.Content;

namespace CYINT.XPlatformNetworking.Android
{
    public class AndroidNetworking : IXPlatformNetworking
    {

        protected Context _appContext;
        protected ConnectivityManager _connectivityManager;

        public AndroidNetworking(Context appContext)
        {
            SetAppContext(appContext);        
        }

        public bool IsNetworkAvailable(Object appContext)
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
                throw new AndroidNetworkingException("Could not get connectivity manager. Context not found.");

            return _connectivityManager;
        }
    }

    public class AndroidNetworkingException : Exception
    {
        public AndroidNetworkingException(string message) : base(message) { }
    }
}