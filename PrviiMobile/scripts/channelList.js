(function () {
    'use strict';
var prviiAppChannelList = angular.module('prviiApp.channelList', []);

prviiAppChannelList.controller('channelListController', ['$rootScope', '$scope', '$http', 'ngProgress',
function ($rootScope, $scope, $http, ngProgress) {
  $scope.show = false;
  ngProgress.start(); 
  var dataToPost = { ID: $rootScope.USER_ID, UserProfileTypeID: $rootScope.USER_PROFILE_TYPE_ID, ChannelID: $rootScope.USER_GroupID };
    $http.post($rootScope.SERVICE_URL + "channel/GetChannelList", dataToPost)
                .success(function (serverResponse, status, headers, config) {
                    $scope.channelList = serverResponse;
                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                }
            );
   ngProgress.complete();
   $scope.show = true;
}]
)
})();