using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYINT.XPlatformNetworking
{
    public interface IXPlatformNetworking 
    {    
        bool IsNetworkAvailable();
        void PlaceInLocalStorage(string key, string content);       
        Task<string> RetrieveFromLocalStorage(string key);
    }
}
