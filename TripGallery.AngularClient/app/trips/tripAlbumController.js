(function () {
    "use strict";
    angular
        .module("tripGallery")
        .controller("tripAlbumController",
                     ["OidcManager", "$scope",
                         TripAlbumController]);

    function TripAlbumController(OidcManager, $scope) {
        var vm = this;        
        vm.address = "";
        vm.mgr = OidcManager.OidcTokenManager();
       
        // call the userinfo endpoint 
        vm.mgr.oidcClient.loadUserProfile(vm.mgr.access_token)
            .then(function (userInfoValues) {
                vm.address = userInfoValues.address;
                $scope.$apply();
        }); 
    }
}());
