# XPlatformNetworking

This library provides a cross platform networking portable class library for mobile applications. It provides a means for accessing RESTFUL-like API's to consume data and also provides a cross platform implementation of local storage to provide a means for caching results when offline.

TODO:
	iOS implementation
	UWP implementation

Install this library as a dependency and then ensure that the proper permissions are added:

    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"></uses-permission>

To use in shared code, get with the DependencyService class

    DependencyService.Get<IXPlatformNetworking>();

To detect whether or not a network connection is present:

   bool hasNetworkAccess = DependencyService.Get<IXPlatformNetworking>().IsNetworkAvailable();

Currently under active development.

Contributors welcome.
