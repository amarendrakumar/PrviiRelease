(function () {
    'use strict';
    var prviiAppchangePassword = angular.module('prviiApp.changePassword', []);

    prviiAppchangePassword.controller('changePasswordController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress', '$location',
    function ($rootScope, $scope, $http, $routeParams, ngProgress, $location) {
       


        $scope.UpdatePassword = function UpdatePassword()
        {
            $scope.show = false;
            ngProgress.start();
            var dataToPost = { ID: $rootScope.USER_ID, Password: $scope.CurrentPassword, NewPassword: $scope.NewPassword };
            $http.post($rootScope.SERVICE_URL + "UserProfile/changePassword", dataToPost)
                  .success(function (serverResponse, status, headers, config) {
                       //alert(serverResponse);
                       if (serverResponse == -1)
                       {
                           alert("Old Password does not match.Please enter correct old password.");
                       }
                       else
                       {
                           alert("Password change succesfully.");
                           $location.path($rootScope.HomePath);
                       }
                     /// $location.path($rootScope.HomePath);
                  }).error(function (serverResponse, status, headers, config) {
                      alert(status + " - Error Occured!");
                  }
              );
            ngProgress.complete();
            $scope.show = true;
        }
        
      
       
    }]
    )

    var directiveId = 'ngMatch';
    prviiAppchangePassword.directive(directiveId, ['$parse', function ($parse) {

        var directive = {
            link: link,
            restrict: 'A',
            require: '?ngModel'
        };
        return directive;

        function link(scope, elem, attrs, ctrl) {
            // if ngModel is not defined, we don't need to do anything
            if (!ctrl) return;
            if (!attrs[directiveId]) return;

            var firstPassword = $parse(attrs[directiveId]);

            var validator = function (value) {
                var temp = firstPassword(scope),
                v = value === temp;
                ctrl.$setValidity('match', v);
                return value;
            }

            ctrl.$parsers.unshift(validator);
            ctrl.$formatters.push(validator);
            attrs.$observe(directiveId, function () {
                validator(ctrl.$viewValue);
            });

        }
    }]);
})();