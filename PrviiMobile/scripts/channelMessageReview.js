(function () {
    'use strict';
    var prviiAppChannelMessageReView = angular.module('prviiApp.channelMessageReview', []);

    prviiAppChannelMessageReView.controller('channelMessageReviewController', ['$rootScope', '$scope', '$http', '$location', '$routeParams', 'ngProgress',
        function ($rootScope, $scope, $http, $location, $routeParams, ngProgress) {

            $scope.show = false;
            ngProgress.start();
           
            // var channelID = $routeParams.channelId;
            var channelID = $rootScope.USER_ChannelID;
           //  debuggerGetChannelMessageList
            var dataToPost = { ID: channelID };
            $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageList", dataToPost)
                        .success(function (serverResponse, status, headers, config) {
                            debugger 
                            $scope.channelMessageList = serverResponse;
                            $scope.channelID = channelID;
                        }).error(function (serverResponse, status, headers, config) {
                            alert(status + " - Error Occured!");
                        }
                    );
            // $scope.channelMessageList = "Amarendra : " + channelID;
            ngProgress.complete();
            $scope.show = true;
            $scope.predicate = '-ScheduledOn';
            $scope.myFilter = function (channelMessage) {
                if (channelMessage.Status === 1)
                    return true;
                //else if (channelMessage.Status === 3)
                //    return true;
                else
                    return false;

            };
            $scope.myFilterSent = function (channelMessage) {
                if (channelMessage.Status === 2)
                    return true;                  
                else
                    return false;
            };
            
            $scope.myFilterPending = function (channelMessage) {
                if (channelMessage.Status === 3)
                    return true;
                else
                    return false;
            };


            $scope.myFilterDelete = function (channelMessage) {
                if (channelMessage.Status === 1)
                {
                   // return true;
                   //// alert(channelMessage.IsDeleted);
                    if (channelMessage.IsDeleted === false)
                        return true;
                    else
                        return false;
                }                                  
                else
                    return false;

            };
            
            $rootScope.includeDeleteView = function (templateURI,messageId) {
                $rootScope.srcDelete = 'partials/' + templateURI;
                $rootScope.delete = true;              
                $rootScope.viewMessageID = messageId;
            }
            $rootScope.includeChannelMessageAllView = function (templateURI, messageId) {
                $rootScope.srcViewAll = 'partials/' + templateURI;
                $rootScope.viewAllMessage = true;
                $rootScope.viewMessageID = messageId;
            }
            
            $rootScope.includeChannelMessageSendView = function (templateURI, messageId) {
                $rootScope.srcViewSent = 'partials/' +templateURI;
                $rootScope.viewSentMessage = true;
                $rootScope.viewMessageID = messageId;
            }
            $rootScope.includeChannelMessagePendingView = function (templateURI, messageId) {
                $rootScope.srcViewPending = 'partials/' + templateURI;
                $rootScope.viewPendingMessage = true;
                $rootScope.viewMessageID = messageId;
            }

            $scope.DeleteMessage = function (messageId, channelId) {
                if (window.confirm('Are you sure?'))
                {
                    $scope.show = false;
                    ngProgress.start();
                    var messageData = { ID: messageId };
                    $http.post($rootScope.SERVICE_URL + "ChannelMessage/ChannelMessageDelete", messageData)
                            .success(function (serverResponse, status, headers, config) {
                                alert("success");
                                var dataToPost = { ID: channelID };
                                $http.post($rootScope.SERVICE_URL + "ChannelMessage/GetChannelMessageList", dataToPost)
                                            .success(function (serverResponse, status, headers, config) {
                                                //debugger
                                                $scope.channelMessageList = serverResponse;
                                                $scope.channelID = channelID;
                                            }).error(function (serverResponse, status, headers, config) {
                                                alert(status + " - Error Occured!");
                                            }
                                        );
                            }).error(function (serverResponse, status, headers, config) {
                                alert(status + " - Error Occured!");
                            }
                        );
                    ngProgress.complete();
                    $scope.show = true;
                }
               
            }


          

        }]
    )
})();