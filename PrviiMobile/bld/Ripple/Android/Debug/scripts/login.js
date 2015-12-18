(function () {
    'use strict';
    var prviiAppLogin = angular.module('prviiApp.login', []);
   
    prviiAppLogin.controller('loginController', ['$scope', '$rootScope', '$http', '$location', 'ngProgress',
function ($scope, $rootScope, $http, $location, ngProgress) {
    //debugger
     //alert("login page");
    $scope.signin = function () {
      // alert($rootScope.SERVICE_URL + "userprofile/authenticate");
      //debugger
        var deviceregistrationID = localStorage.getItem('deviceregistrationid');
        $scope.isSaving = true;
        $scope.show = false;
        ngProgress.start();
        var DeviceIfo = { ID: 0, SubscriberId: 0, DeviceCordova: device.cordova, DevicePaltform: device.platform, DeviceId: device.uuid, DeviceVersion: device.version, IsActive: true }
       // debugger
        var dataToPost = { ID: 0, Email: $scope.login.Username, Password: $scope.login.Password, userDeviceInfo: DeviceIfo };
        $http.post($rootScope.SERVICE_URL + "userprofile/authenticate", dataToPost)
                .success(function (serverResponse, status, headers, config) {
                // alert("success");
                 //  debugger
                   var dataToPostDeviceToken = { SubscriberId: serverResponse.ID, DeviceId: device.uuid, DeviceToken: deviceregistrationID };
                    $http.post($rootScope.SERVICE_URL + "userprofile/SaveDeviceToken", dataToPostDeviceToken)
                            .success(function (serverResponse, status, headers, config) {
                                //alert("devicetoken save " + success);
                            }).error(function (serverResponse, status, headers, config) {
                                //alert(status + " - Error Occured! device token");                                
                            })

                    var id = serverResponse.ID;
                    if (id > 0) {
                        $rootScope.Authenticate = false;
                        $rootScope.USER_ID = serverResponse.ID;
                        $rootScope.USER_EMAIL = serverResponse.Email;
                        $rootScope.Full_NAME = serverResponse.Firstname + ' ' + serverResponse.Lastname;
                        $rootScope.USER_FIRST_NAME = serverResponse.Firstname;
                        $rootScope.USER_LAST_NAME = serverResponse.Lastname;
                        $rootScope.USER_PROFILE_TYPE_ID = serverResponse.UserProfileTypeID;
                        $rootScope.USER_ChannelID = serverResponse.ChannelID;
                        $rootScope.USER_GroupID = serverResponse.GroupID;
                        $rootScope.USER_NickName = serverResponse.NickName;

                        var dataToPostProfileType = { ID: $rootScope.USER_ID };
                        $http.post($rootScope.SERVICE_URL + "userprofile/GetUserProfileTypeByUserID", dataToPostProfileType)
                                 .success(function (serverResponse, status, headers, config) {
                                   //  debugger
                                     $rootScope.UserProfileTypeList = serverResponse;
                                       // debugger
                                     if ($rootScope.UserProfileTypeList.length == 1) {
                                         $rootScope.USER_PROFILE_TYPE_ID = $rootScope.UserProfileTypeList[0].ProfileTypeID;
                                         $rootScope.Authenticate = true;
                                         if ($rootScope.USER_PROFILE_TYPE_ID === 3)
                                         {
                                             var dataToPostChannel = { ID: $rootScope.USER_ChannelID, UserID: $rootScope.USER_ID };
                                             $http.post($rootScope.SERVICE_URL + "channel/GetChannelByID", dataToPostChannel)
                                                         .success(function (serverResponse, status, headers, config) {
                                                       //  debugger
                                                             $scope.channelDetails = serverResponse;
                                                             $rootScope.IsSubscriber = false;
                                                             $rootScope.IsCelebrityUser = true;
                                                         $rootScope.USER_ChannelName = $scope.channelDetails.Firstname + ' ' + $scope.channelDetails.Lastname;
                                                             $rootScope.HomePath = '/channelDetails/' + $scope.channelDetails.ID;
                                                             $location.path('/channelDetails/' + $scope.channelDetails.ID);
                                                         }).error(function (serverResponse, status, headers, config) {
                                                            // alert(status + " - Error Occured!");
                                                             $location.path('/logout');
                                                         }
                                                     );
                                            
                                         }
                                         else if ($rootScope.USER_PROFILE_TYPE_ID === 4)
                                         {
                                             $rootScope.IsSubscriber = true;
                                             $rootScope.IsCelebrityUser = false;
                                             $rootScope.HomePath = '/home';
                                             $location.path('/home');
                                         }                                       
                                         else
                                         {
                                             $rootScope.HomePath = '/home';
                                             $location.path('/home');
                                         }                                        
                                        
                                     }
                                     else
                                         $location.path('/ProfileType');


                                 }).error(function (serverResponse, status, headers, config) {
                                     //alert(status + " - Error Occured!");
                                     $location.path('/logout');
                                 })
                        

                    }
                    else {
                        alert("Invalid User");
                        $location.path('/logout');
                    }
                    //debugger
                    ngProgress.complete();
                    $scope.show = true;
                    $scope.isSaving = false;
                }).error(function (serverResponse, status, headers, config) {
                    //debugger
                    $rootScope.Authenticate = false;
                    //alert(status + " - Error Occured!");
                    ngProgress.complete();
                    $scope.show = true;
                    $scope.isSaving = false;
                }

            );

    };

    $scope.LoginContinue = function () {
        if ($scope.profiletype.ProfileTypeID != null) {
            $rootScope.Authenticate = true;
            $rootScope.USER_PROFILE_TYPE_ID = $scope.profiletype.ProfileTypeID;
            if ($rootScope.USER_PROFILE_TYPE_ID === 3) {
                var dataToPostChannel = { ID: $rootScope.USER_ChannelID, UserID: $rootScope.USER_ID };
                $http.post($rootScope.SERVICE_URL + "channel/GetChannelByID", dataToPostChannel)
                            .success(function (serverResponse, status, headers, config) {
                                $scope.channelDetails = serverResponse;
                                $rootScope.IsCelebrityUser = true;
                                $rootScope.IsSubscriber = false;
                                $rootScope.USER_ChannelName = $scope.channelDetails.Firstname + ' ' + $scope.channelDetails.Lastname;
                                $rootScope.HomePath = '/channelDetails/' + $scope.channelDetails.ID;
                                $location.path('/channelDetails/' + $scope.channelDetails.ID);
                            }).error(function (serverResponse, status, headers, config) {
                                alert(status + " - Error Occured!");
                                $location.path('/logout');
                            }
                        );              
            }
            else if ($rootScope.USER_PROFILE_TYPE_ID === 4) {
                $rootScope.IsSubscriber = true;
                $rootScope.HomePath = '/home';
                $location.path('/home');
            }
            else {
                $rootScope.HomePath = '/home';
                $location.path('/home');
            }
           
        }
        else {
            alert("please select user profile type.")
        }

    }


    $scope.LoginCancel = function () {
        $location.path('/logout');
    }
    $scope.Back = function () {
        $location.path('/');
    }

    

   
  
  
    
}]);

})();