﻿/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeValue.VSCopyLocal;

namespace VSCopyLocal_UnitTests
{
    [TestClass]
    public class PackageTest
    {

        [TestMethod]
        public void IsIVsPackage()
        {
            var package = new VSCopyLocalPackage();
            Assert.IsNotNull(package, "The object does not implement IVsPackage");
        }

        [TestMethod]
        public void SetSite()
        {
            // Create the package
            IVsPackage package = new VSCopyLocalPackage();
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}
