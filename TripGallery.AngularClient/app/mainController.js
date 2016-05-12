(function () {
    "use strict";
    angular
        .module("tripGallery")      
        .controller("mainController",
                     ["OidcManager", MainController]);
 

    function MainController(OidcManager) {
        var vm = this;

        vm.logOut = function () {
            vm.mgr.removeToken();
            window.location = "index.html";
        }

        vm.logOutOfIdSrv = function () {
            vm.mgr.redirectForLogout();
        } 
      
        vm.mgr = OidcManager.OidcTokenManager();

        // no id token or expired => redirect to get one
        if (vm.mgr.expired) {
            debugger;
           // alert("sdfsdfdsfd");
            vm.mgr.redirectForToken();
        } 
    }

}());
