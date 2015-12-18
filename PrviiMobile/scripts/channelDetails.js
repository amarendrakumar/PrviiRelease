(function () {
    'use strict';
    var prviiAppchannelDetails = angular.module('prviiApp.channelDetails', []);

    prviiAppchannelDetails.controller('channelDetailsController', ['$rootScope', '$scope', '$http', '$routeParams','ngProgress',
    function ($rootScope, $scope, $http, $routeParams, ngProgress) {
        $scope.show = false;
        ngProgress.start();
        //debugger
        var channelID = $routeParams.channelId;
        //$rootScope.channelId = channelID;
        $rootScope.USER_ChannelID = channelID;
        //alert(channelID);
        var dataToPost = { ID: channelID ,UserID: $rootScope.USER_ID, UserProfileTypeID: $rootScope.USER_PROFILE_TYPE_ID};
        $http.post($rootScope.SERVICE_URL + "channel/GetChannelByID", dataToPost)
                    .success(function (serverResponse, status, headers, config) {                       
                        $scope.channelDetails = serverResponse;
                        $rootScope.srcView = 'partials/channelFullDetails.html';
                    }).error(function (serverResponse, status, headers, config) {                       
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;


       

        $rootScope.view = false;
        $rootScope.event = false;
        $rootScope.message = false;
        $rootScope.reports = false;

        
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

        $rootScope.includeEvent = function (templateURI, isTrue) {
            $rootScope.srcEvent = 'partials/' +templateURI;

            if (isTrue)
                $rootScope.event = false;
            else
                $rootScope.event = true;

            $rootScope.message = false;
            $rootScope.reports = false;
            $rootScope.view = false;
        }



        $rootScope.includeMessage = function (templateURI, isTrue) {
            $rootScope.srcMessage = 'partials/' +templateURI;
            if (isTrue)
            {
                $rootScope.message = false;
            }
            else
            {
                $rootScope.message = true;
            }
              
            $rootScope.srcModify = "";
            $rootScope.srcDelete = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";

            $rootScope.view = false;
            $rootScope.reports = false;
            $rootScope.event = false;
        }

        $rootScope.includeReports = function (templateURI, isTrue) {
           // $rootScope.srcReports = 'partials/' +templateURI;

            if (isTrue)
            {
                $rootScope.reports = false;
               // $rootScope.statistics = false;
             }
            else
            {
                $rootScope.reports = true;
               // $rootScope.statistics = true;
            }
               

            $rootScope.view = false;
            $rootScope.message = false;
            $rootScope.event = false;
        }


        $rootScope.create = false;
        $rootScope.review = false;
        $rootScope.modify = false;
        $rootScope.delete = false;
        $rootScope.viewAll = false;

       
        $rootScope.includeCreate = function (templateURI, isTrue) {
            $rootScope.srcMessage = 'partials/' +templateURI;

            if (isTrue)
                $rootScope.create = false;
            else
                $rootScope.create = true;

            $rootScope.srcModify = "";
            $rootScope.srcDelete = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";

            $rootScope.review = false;
            $rootScope.modify = false;
            $rootScope.delete = false;
            $rootScope.viewAll = false;
        }


        //$rootScope.includeReview = function (templateURI, isTrue) {
        //    $rootScope.srcReview = 'partials/' + templateURI;

        //    if (isTrue)
        //        $rootScope.review = false;
        //    else
        //        $rootScope.review = true;

           

        //    $rootScope.create = false;
        //    $rootScope.modify = false;
        //    $rootScope.delete = false;
        //    $rootScope.viewAll = false;
        //}

        $rootScope.includeModify = function (templateURI, isTrue) {
           
            $rootScope.srcModify = 'partials/' + templateURI;
            
            $rootScope.srcMessage = "";
            $rootScope.srcDelete = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";

            if (isTrue)
                $rootScope.modify = false;
            else
                $rootScope.modify = true;

            $rootScope.create = false;
            $rootScope.review = false;
            $rootScope.delete = false;
            $rootScope.viewAll = false;
        }


        $rootScope.includeDelete = function (templateURI, isTrue) {
            $rootScope.srcDelete = 'partials/' + templateURI;

            $rootScope.srcMessage = "";
            $rootScope.srcModify = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";

            if (isTrue)
                $rootScope.delete = false;
            else
                $rootScope.delete = true;

            $rootScope.create = false;
            $rootScope.review = false;
            $rootScope.modify = false;
            $rootScope.viewAll = false;
        }


       
        $rootScope.viewPendingMessage = false;
        $rootScope.viewSentMessage = false;
        $rootScope.viewAllMessage = false;

        $rootScope.includeViewAll = function (templateURI, isTrue) {
           // debugger
           // $rootScope.srcViewAll = 'partials/' +templateURI;
            //alert("aa");
            $rootScope.srcMessage = "";
            $rootScope.srcModify = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";
            $rootScope.srcDelete = "";

            if (isTrue)
                $rootScope.viewAll = false;
            else
                $rootScope.viewAll = true;

            $rootScope.create = false;
            $rootScope.review = false;
            $rootScope.modify = false;
            $rootScope.delete = false;

            $rootScope.viewPendingMessage = false;
            $rootScope.viewSentMessage = false;
            $rootScope.viewAllMessage = false;
        }

        $rootScope.includePending = function (templateURI, isTrue) {            
            $rootScope.srcViewPending = 'partials/' + templateURI;

            $rootScope.srcMessage = "";
            $rootScope.srcModify = "";            
            $rootScope.srcViewSent = "";
            $rootScope.srcViewAll = "";
            $rootScope.srcDelete = "";

            if (isTrue)
                $rootScope.viewPendingMessage = false;
            else
                $rootScope.viewPendingMessage = true;

            $rootScope.viewAll = true;
            $rootScope.viewSentMessage = false;
            $rootScope.viewAllMessage = false;
        }

        $rootScope.includeSent = function (templateURI, isTrue) {
            $rootScope.srcViewSent = 'partials/' + templateURI;

            $rootScope.srcMessage = "";
            $rootScope.srcModify = "";
            $rootScope.srcViewPending = "";
           
            $rootScope.srcViewAll = "";
            $rootScope.srcDelete = "";

            if (isTrue)
                $rootScope.viewSentMessage = false;
                else
                $rootScope.viewSentMessage = true;

            $rootScope.viewAll = true;
            $rootScope.viewPendingMessage = false;
            $rootScope.viewAllMessage = false;
         }

        $rootScope.includeAll = function (templateURI, isTrue) {
            $rootScope.srcViewAll = 'partials/' + templateURI;

            $rootScope.srcMessage = "";
            $rootScope.srcModify = "";
            $rootScope.srcViewPending = "";
            $rootScope.srcViewSent = "";
           
            $rootScope.srcDelete = "";

            if (isTrue)
                $rootScope.viewAllMessage = false;
                else
                $rootScope.viewAllMessage = true;

            $rootScope.viewAll = true;
            $rootScope.viewPendingMessage = false;
            $rootScope.viewSentMessage = false;
                }
        //////////// Report ////////////////////////

        $rootScope.statistics = false;
        $rootScope.subscriber = false;
        $rootScope.revenue = false;
     
        $rootScope.includeSubscriber = function (templateURI, isTrue) {
            $rootScope.srcSubscriber = 'partials/' +templateURI;

            if (isTrue)
                $rootScope.subscriber = false;
            else
                $rootScope.subscriber = true;

            $rootScope.statistics = false;
            $rootScope.revenue = false;
           
        }

         $rootScope.includeStatistics = function (templateURI, isTrue) {
             $rootScope.srcReports = 'partials/' + templateURI;

             if (isTrue)
                 $rootScope.statistics = false;
             else
                 $rootScope.statistics = true;

             $rootScope.subscriber = false;
             $rootScope.revenue = false;

         }

         $rootScope.includeRevenue = function (templateURI, isTrue) {
             $rootScope.srcRevenue = 'partials/' + templateURI;

             if (isTrue)
                 $rootScope.revenue = false;
             else
                 $rootScope.revenue = true;

             $rootScope.subscriber = false;
             $rootScope.statistics = false;

         }

    }]
    )

    prviiAppchannelDetails.filter('unsafe', function ($sce) {
        return function (val) {
            return $sce.trustAsHtml(val);
        };
    });

})();