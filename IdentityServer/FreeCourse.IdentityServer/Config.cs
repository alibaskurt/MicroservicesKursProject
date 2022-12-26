// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        //JWT payload içerisinde aud , scope paremetreleri yer alıcak. 
        //Aud parametresi bu jwt token ile clientın hangi microservislere istek yapabileceği bilgisi yer alıcak.
        //Scope ıcersınde ıse mıcroservıse ıstek yaparken yetkısı yer alıcak read,write,fullpermission gibi.
        //Burada roller ve izinler ile ilgili işlemler de yapılır.
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog")
            {
                Scopes={ "catalog_fullpermission" }
            },
            new ApiResource("resource_photo_stock")
            {
                Scopes={ "photo_stock_fullpermission" }
            },

            //Signup metoduna erişim için scope tanımladık. //Identity server içindeki defoult tanımlamayı yaptık.
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            {
                Scopes = { IdentityServerConstants.LocalApi.ScopeName }
            }
        };

        //Identity Server microservisi için token doğruladıktan sonra kullanıcıya ait erişilebilecek alanlar.
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.Email(),
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource()
            {
                Name = "roles",
                DisplayName = "Roles",
                Description = "Kullanıcı Rolleri",
                UserClaims = new []{"role"}
            }

        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","Catalog API için full erişim"),
                new ApiScope("photo_stock_fullpermission","Photo Stock API için full erişim"),
                //Sigup için oluşturulan scope
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName,"Asp.net mvc clientının sign up methoduna full erişim için tanımlandı")
            };

        //Token almak isteyen client tanımlaması burada yapılır.
        //Client buraya ClientId ve ClientSecret değeri ile gelicek. Hangi microservislere bu token ile erişicek bu ayarlama yapıldı.
        //Client Cridential da refresh token yok. Resourse Owner da refresh token var.
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "ASP Net Core MVC",
                    ClientId = "WebMvcClient",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes =
                    {
                        new string(GrantType.ClientCredentials)
                    },

                    //Scope lar hangi aud a aitse token içerisine o değeri basar.
                    AllowedScopes =
                    {
                        "catalog_fullpermission",
                        "photo_stock_fullpermission",
                        IdentityServerConstants.LocalApi.ScopeName
                    }

                },
                new Client
                {
                    ClientName = "ASP Net Core MVC",
                    ClientId = "WebMvcClientUser",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes =
                    {
                        new string(GrantType.ResourceOwnerPassword)
                    },

                    //Kullanıcı logın olmasada refresh token ıle access token alınabilsin.
                    AllowOfflineAccess = true,

                    //Scope lar hangi aud a aitse token içerisine o değeri basar.
                    AllowedScopes =
                    {
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.Email,
                        //Mutlaka kullanıcı Id olması gerekiyor.
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        //Refresh token donebilmek için tanımlandı. Kullanıcı o an login olmamıs olsada refresh token ile token alabiliriz.
                        IdentityServerConstants.StandardScopes.OfflineAccess 
                    },
                    //Access Token 1 saatliğine verilir.
                    AccessTokenLifetime = 1*60*60,
                    RefreshTokenExpiration = TokenExpiration.Absolute,

                    //Refresh token ömrü 1 ay.
                    //Her access token alındığında refresh token da dönülecek ve refresh tokenın omru artık yenıden 1 ay olucak
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(30)-DateTime.Now).TotalSeconds,

                    //Refresh token 1 kere kullanılmasın tekrar kullanılabilir olsun.
                    RefreshTokenUsage = TokenUsage.ReUse

                }
            };
    }
}