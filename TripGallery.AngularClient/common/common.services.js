(function () {
    "use strict";

    angular.module("common.services",
                    ["ngResource"])
      	.constant("appSettings",
        {
            tripGalleryAPI: "https://localhost:44315" 
        });


    // oidc manager for dep injection
    angular.module("common.services")
        .factory("OidcManager", function () {

            //// configure manager
            //var config = {
            //    client_id: "tripgalleryimplicit",
            //    redirect_uri:  window.location.protocol + "//" + window.location.host + "/callback.html",
            //    response_type: "id_token token",
            //    scope: "openid profile address gallerymanagement roles",               
            //    authority: "https://localhost:44317/identity"
            //};

            // configure manager with post logout redirect
            //var config = {
            //    client_id: "tripgalleryimplicit",
            //    redirect_uri: window.location.protocol + "//" + window.location.host + "/callback.html",
            //    post_logout_redirect_uri: window.location.protocol + "//" + window.location.host + "/index.html",
            //    response_type: "id_token token",
            //    scope: "openid profile address gallerymanagement roles",
            //    authority: "https://localhost:44317/identity" 
            //};

            //// configure manager, including session management support
            var config = {
                client_id: "tripgalleryimplicit",
                redirect_uri: window.location.protocol + "//" + window.location.host + "/callback.html",
                post_logout_redirect_uri: window.location.protocol + "//" + window.location.host + "/index.html",
                response_type: "id_token token",
                scope: "openid profile address gallerymanagement roles",
                authority: "https://localhost:44317/identity",
                silent_redirect_uri: window.location.protocol + "//" + window.location.host + "/silentrefreshframe.html",
                silent_renew: true,
                acr_values: "2fa"
            };
                    
            var mgr = new OidcTokenManager(config);
             
            return {
                OidcTokenManager: function () {
                    return mgr;
                } 
        };
    });


}());
