# XPlatformNetworking

This library provides a cross platform networking portable class library for mobile applications. It provides a means for accessing RESTFUL-like API's to consume data and also provides a cross platform implementation of local storage to provide a means for caching results when offline.

For Android, install this library as a dependency and then ensure that the proper permissions are added to the manifest:

    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"></uses-permission>

To use, simply pass the context into the AndroidNetworking constructor within a context such as an Activity.

    AndroidNetworking YourClassVariable = new AndroidNetworking(this);

To detect whether or not a network connection is present:

   bool hasNetworkAccess = YourClassVariable.IsNetworkAvailable();

Currently under active development.

Contributors welcome.
