(function () {
    'use strict';
    var prviiAppsubscriberChannelMessageview = angular.module('prviiApp.subscriberChannelMessageview', []);

    prviiAppsubscriberChannelMessageview.controller('subscriberChannelMessageviewController', ['$rootScope', '$scope', '$http', '$routeParams', 'ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {

       // var channelID = $rootScope.channelId;

        $scope.show = false;
        ngProgress.start();
        //debugger
        var channelID = $routeParams.channelId;
        //var channelID = $rootScope.channelId;
       // $rootScope.channelId = channelID;
        alert(channelID);
        var dataToPost = { ID: channelID, UserID: $rootScope.USER_ID, UserProfileTypeID: $rootScope.USER_PROFILE_TYPE_ID };
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

        $rootScope.channelviewFull = false;
        $rootScope.subscribermessage = false;

        $rootScope.messagePastWeek = false;
        $rootScope.MessageSearch = false;
        $rootScope.viewAllMessage = false;

        $rootScope.includeSubscriberChannelFullDetailsMessage = function (templateURI, isTrue) {
           // $rootScope.srcChennalViewFull = "";

            $rootScope.srcChennalViewFull = 'partials/' + templateURI;

            if (isTrue) {
                $rootScope.channelviewFull = false;
            }
            else {
                $rootScope.channelviewFull = true;
            }

            $rootScope.subscribermessage = false;
            $rootScope.messagePastWeek = false;
            $rootScope.MessageSearch = false;
            $rootScope.viewAllMessage = false;

        }

        $scope.includeMessage = function (isTrue) {

            if (isTrue) {
                $scope.subscribermessage = false;
            }
            else {
                $scope.subscribermessage = true;
            }


            $rootScope.channelviewFull = false;
            $rootScope.messagePastWeek = false;
            $rootScope.MessageSearch = false;
            $rootScope.viewAllMessage = false;

        }



        $rootScope.includeSubscriberMessagePastWeek = function (templateURI, isTrue) {
          //  $rootScope.srcSubscriberMessagePastWeek = "";

            $rootScope.srcSubscriberMessagePastWeek = 'partials/' + templateURI;
            if (isTrue) {
                $rootScope.messagePastWeek = false;
            }
            else {
                $rootScope.messagePastWeek = true;
            }

            $rootScope.channelviewFull = false;
            $rootScope.subscribermessage = false;
            $rootScope.MessageSearch = false;
            $rootScope.viewAllMessage = false;
        }

        $rootScope.includeViewAllMessage = function (templateURI, isTrue) {
            $rootScope.srcviewAllMessage = "";
            $rootScope.srcviewAllMessage = 'partials/' + templateURI;
            if (isTrue) {
                $rootScope.viewAllMessage = false;
            }
            else {
                $rootScope.viewAllMessage = true;
            }

            $rootScope.channelviewFull = false;
            $rootScope.subscribermessage = false;
            $rootScope.MessageSearch = false;
            $rootScope.messagePastWeek = false;

        }



        $rootScope.includeSubscriberMessageSearch = function (templateURI, isTrue) {
           // alert("amat");

            $rootScope.srcSubscriberMessageSearch = 'partials/' + templateURI;
            if (isTrue) {
                $rootScope.MessageSearch = false;
            }
            else {
                $rootScope.MessageSearch = true;
            }

            $rootScope.channelviewFull = false;
            $rootScope.subscribermessage = false;           
            $rootScope.messagePastWeek = false;
            $rootScope.viewAllMessage = false;

          
        }



        $rootScope.includeSubscriberMessageDetailsViewPastWeek = function (templateURI, messageId) {
            $rootScope.messageId = messageId;
            $rootScope.srcSubscriberMessagePastWeek = 'partials/' + templateURI;
            $rootScope.messagePastWeek = true;

        }
        $rootScope.includeSubscriberMessageDetailsViewAll = function (templateURI, messageId) {
            $rootScope.messageId = messageId;
            $rootScope.srcviewAllMessage = 'partials/' + templateURI;
            $rootScope.viewAllMessage = true;

        }

        $rootScope.includeSubscriberMessageDetailsViewSearch = function (templateURI, messageId) {
            $rootScope.messageId = messageId;
            $rootScope.srcSubscriberMessageSearch = 'partials/' + templateURI;
            $rootScope.MessageSearch = true;

        }

    }]
    )
})();