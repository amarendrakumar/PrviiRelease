(function () {
    'use strict';
    var prviiAppchannelDetails = angular.module('prviiApp.subscriberChannelDetails', []);

    prviiAppchannelDetails.controller('subscriberChannelDetailsController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        //debugger

        var channelID = $rootScope.channelId;
       
        //alert(channelID);
        var dataToPost = { ID: channelID ,UserID: $rootScope.USER_ID, UserProfileTypeID: $rootScope.USER_PROFILE_TYPE_ID};
        $http.post($rootScope.SERVICE_URL + "channel/GetChannelByID", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                       // debugger
                        $scope.channelDetails = serverResponse;
                    }).error(function (serverResponse, status, headers, config) {
                       // debugger
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;


       

     
        
        $rootScope.includeView = function (templateURI, isTrue) {
            $rootScope.srcView = 'partials/' + templateURI;

            if (isTrue)
                $rootScope.view = false;
            else
                $rootScope.view = true;

            $rootScope.message = false;
            $rootScope.reports = false;
            $rootScope.event = false;
        }

      
    }]
    )


})();