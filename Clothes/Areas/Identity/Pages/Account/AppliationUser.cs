// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Identity;

namespace ClothesApp.Areas.Identity.Pages.Account
{
    internal class AppliationUser : IdentityUser
    {
        internal string Surname;
        internal string PostaKodu;
        internal string Role;

        public string Adres { get; internal set; }
        public string Sehir { get; internal set; }
        public string Semt { get; internal set; }
        public string Name { get; internal set; }
    }
}