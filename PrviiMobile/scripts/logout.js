(function () {
    'use strict';
    var prviiAppLogout = angular.module('prviiApp.logout', []);

    prviiAppLogout.controller('logoutController', ['$rootScope', '$scope', '$location',
        function ($rootScope, $scope, $location) {
            $rootScope.Authenticate = false;
            $rootScope.USER_ID = 0;
            $rootScope.USER_NAME = null;
            $rootScope.PASSWORD = null;
            $rootScope.USER_PROFILE_TYPE_ID = 0;
            $rootScope.GetLoggedUser = null;
            $rootScope.USER_ChannelID =0;
            $rootScope.USER_GroupID = 0;
            $rootScope.USER_NickName = null;
            $rootScope.USER_ChannelName = null;
            $rootScope.IsCelebrityUser = false;
            $location.path('/login');

          
        }]);


})();