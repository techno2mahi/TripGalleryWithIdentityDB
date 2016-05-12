(function () {
    "use strict";
    angular
        .module("tripGallery")     
        .controller("pictureIndexController",
                     ["pictureResource",
                         "$routeParams", "OidcManager",
                         PictureIndexController]);

    function PictureIndexController(pictureResource, $routeParams, OidcManager) {
        var vm = this;

        vm.tripId = $routeParams.tripId;

        vm.mgr = OidcManager.OidcTokenManager();
        vm.getUserIdentifier = function () {
            // combine issuer + sub
            return "https://tripcompanysts/identity" + vm.mgr.profile.sub;
        }
        
        vm.deletePicture = function (tripId, id) {
            // delete picture - non-get (query, get) methods are
            // mapped onto a resource instance - we need to get
            // the picture resource from the list.
            
            var pictureToDelete = new pictureResource();

            pictureToDelete.$delete(
                {
                    tripId: tripId,
                    pictureId: id
                },
                 // success: reload pictures
                function () {                   
                    vm.loadPictures(tripId);
                }
                ,
                // failure
                null);
        };

        // function to query the resource - returns list of pictures.  
        // (used at start & after delete)
        vm.loadPictures = function (tripId) {
            // query => get array
            pictureResource.query(
                // params
                {
                    tripId: vm.tripId
                },
                // success
                   function (data) {
                       vm.pictures = data;
                   }
                   // failure
                   ,
                   null
               );
        };

      
        // trigger load
        vm.loadPictures(vm.tripId);


    }


}());
