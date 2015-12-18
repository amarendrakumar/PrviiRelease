(function () {
    'use strict';
    var prviiAppHeader = angular.module('prviiApp.header', []);

    prviiAppHeader.controller('headerController', ['$rootScope', '$scope', '$location',
        function ($rootScope, $scope, $location) {
            //$rootScope.Authenticate = false;
            //$rootScope.USER_ID = 0;
            //$rootScope.USER_NAME = null;
            //$rootScope.PASSWORD = null;
            //$rootScope.USER_PROFILE_TYPE_ID = 0;
            //$rootScope.GetLoggedUser = null;
            //$location.path('/login');

            //$scope.GoHome=function()
            //{
            //    alert('aaa');
            //    $scope.group = false;
            //    $scope.celeberity = false;
            //    $scope.subscriber = false;
            //    $scope.contact = false;
            //    $location.path('/home');
            //}
         
            //alert($rootScope.HomePath);
           // alert(device.uuid);

            $scope.Home = function () {
                $location.path($rootScope.HomePath);
            }


            $scope.ViewProfile = function () {
                $location.path('/ViewProfile');
            }

            $scope.changePassword = function () {
                $location.path('/changePassword');
            }
            $scope.logout = function () {
                //debugger
                //var objDeviceIfo = {   DeviceId: device.uuid ,SubscriberId: $rootScope.USER_ID}
                //$http.post($rootScope.SERVICE_URL + "userprofile/logoutDevice", objDeviceIfo)
                //        .success(function (serverResponse, status, headers, config) {
                //            alert(serverResponse)

                //            debugger
                //            $location.path('/logout');
                //        }).error(function (serverResponse, status, headers, config) {
                //            alert(status + " - Error Occured!");
                //            $location.path('/logout');
                //        })

                $location.path('/logout');
            }


            $scope.showmenu = function (isTrue) {
                // alert('aaa');
                if (!isTrue) {
                    $scope.menu = true;
                    $scope.isTrue = false;
                }
                else {
                    $scope.menu = false
                    $scope.isTrue = true;
            }

            };
            $scope.Hidemenu = function () {
                //alert('aaa hide');
                $scope.menu = false
            };
        }]);


})();