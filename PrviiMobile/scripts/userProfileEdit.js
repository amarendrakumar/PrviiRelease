(function () {
    'use strict';
    var prviiAppuserProfileEdit = angular.module('prviiApp.userProfileEdit', []);

    prviiAppuserProfileEdit.controller('userProfileEditController', ['$rootScope', '$scope', '$location', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $location, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        $http.post($rootScope.SERVICE_URL + "UserProfile/GetCountry")
                 .success(function (serverResponse, status, headers, config) {
                     $scope.countrys = serverResponse;
                 }).error(function (serverResponse, status, headers, config) {
                     alert(status + " - Error Occurred!");
                 });

   

        //$http.post($rootScope.SERVICE_URL + "UserProfile/GetCity")
        //           .success(function (serverResponse, status, headers, config) {
        //               $scope.cities = serverResponse;
        //           }).error(function (serverResponse, status, headers, config) {
        //               alert(status + " - Error Occurred!");
        //           });

        var dataToPost = { ID: $rootScope.USER_ID };
        $http.post($rootScope.SERVICE_URL + "UserProfile/GetUserProfileById", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        //  $scope.userDetails = serverResponse;
                       //  debugger
                        $scope.NickName = serverResponse.NickName;
                        $scope.Firstname = serverResponse.Firstname;
                        $scope.Lastname = serverResponse.Lastname;
                        $scope.Email = serverResponse.Email;
                        $scope.Telephone = serverResponse.Telephone;
                        $scope.Mobile = serverResponse.Mobile;
                        $scope.Country = serverResponse.Country;
                        $scope.State = serverResponse.State;
                        $scope.City = serverResponse.City;
                        $scope.ZipCode = serverResponse.ZipCode;
                        $scope.Address1 = serverResponse.Address1;
                        $scope.Address2 = serverResponse.Address2;
                        $scope.CountryId = serverResponse.CountryId;
                        $scope.StateId = serverResponse.StateId;
                        $scope.Country = serverResponse.Country;
                        //debugger
                       
                        //$scope.country.ID= serverResponse.CountryId;
                        $scope.DeliveryMethod = serverResponse.DeliveryMethod;
                        $scope.Password = serverResponse.Password;
                        $scope.UserProfileTypeID = serverResponse.UserProfileTypeID;
                       // debugger
                        $scope.GetState(serverResponse.CountryId);
                        $scope.country = $scope.CountryId
                        $scope.state = $scope.StateId

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occurred!");
                    }
                );

      

        ngProgress.complete();
        $scope.show = true;

        $scope.GetState = function (CountryId) {          
            $http.post($rootScope.SERVICE_URL + "UserProfile/GetStateByCountryID?CountryID=" +CountryId)
            .success(function (serverResponse, status, headers, config) {               
                $scope.states = serverResponse;
                }).error(function (serverResponse, status, headers, config) {
                debugger
                alert(status + " - Error Occurred!");
                });
        }


        $scope.changeStateBind = function () {
            if ($scope.country > 0)
                $scope.GetState($scope.country);
        }

        $scope.UpdateUserDetails = function ()
                    {
                    // alert("success.....");
                   /// debugger
            $scope.show = false;
            ngProgress.start();
            $scope.countryID = $scope.country;
            $scope.stateId = $scope.state;
                //$scope.CityId = $scope.city.ID;
            var dataToPost = {
                ID: $rootScope.USER_ID, NickName: $scope.NickName, Firstname: $scope.Firstname, Lastname: $scope.Lastname, Email: $scope.Email
                , Telephone: $scope.Telephone, Mobile: $scope.Mobile, ZipCode: $scope.ZipCode, Address1: $scope.Address1, Address2: $scope.Address2
                , CountryId: $scope.country, StateId: $scope.stateId, City: $scope.City, DeliveryMethod: $scope.DeliveryMethod, UserProfileTypeID: $scope.UserProfileTypeID
                , Password: $scope.Password
                };
            $http.post($rootScope.SERVICE_URL + "UserProfile/ManageUserMaster", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                    // alert(serverResponse);                           
                            $location.path('/ViewProfile');
                            ngProgress.complete();
                            $scope.show = true;

                            }).error(function (serverResponse, status, headers, config) {
                            alert(status + " - Error Occurred!");
                            ngProgress.complete();
                            $scope.show = true;
                        }
                    );
        }

    }]
    )
})();