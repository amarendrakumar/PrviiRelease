(function () {
    'use strict';

    //Declare App
    var prviiApp = angular.module('prviiApp', [
       //'ngCordova',
      'ngRoute',      
      'ngAnimate',
      'prviiApp.home',     
      'prviiApp.login',
      'prviiApp.header',
      'prviiApp.logout',
      'prviiApp.channelList',  
      'prviiApp.channelDetails',
      'prviiApp.channelMessageCreate',
      'prviiApp.channelMessageList',
      'prviiApp.channelMessageView',
      'prviiApp.channelSubscriberList',
      'prviiApp.channelSubscriberDetails',
      'prviiApp.subscriberChannelList',
      'prviiApp.newChannelsList',
      'prviiApp.channelSubscriberMessageList',
      'prviiApp.channelSubscriberStatistics',
      'prviiApp.channelMessageEdit',
      'prviiApp.progressApp',
      'prviiApp.userProfile',
      'prviiApp.userProfileEdit',
      'prviiApp.channelMessageReview',
      'prviiApp.messageEditReview',
      'prviiApp.subscriberChannelMessage',
      'prviiApp.subscriberMessageView',
      'prviiApp.channelSubscriberAllMessageList',
      'prviiApp.channelSubscriberPastWeekMessageList',
      'prviiApp.PayPal',
      'prviiApp.changePassword',
      'prviiApp.channelRevenue',
      'prviiApp.subscriberChannelDetails',
      'prviiApp.shoppingCart',
      'prviiApp.appleIndex',
      'prviiApp.channelInApp',
      'prviiApp.subscriberChannelMessageview'
     
    ]);
    
  
    //Define routing
     prviiApp.config(['$routeProvider',
      function ($routeProvider) {
          //debugger
          //var devicePlatform = device.platform;
          var url = 'partials/login.html';
          var controlar = 'loginController';
          //if (devicePlatform == "Android") {
          //    url = 'partials/login.html';
          //    controlar = 'loginController';
          //}
          //else {
          //    url = 'partials/appleIndex.html';
          //    controlar = 'appleIndexController';
          //}
         // alert("ios");
          $routeProvider
               .when('/',
              {
                  templateUrl: url,
                  controller: controlar
              })
              .when('/InAppChannelList',
            {
                templateUrl: 'partials/channelInApp.html',
                controller: 'channelInAppController'
            })
               .when('/AppleRegistration',
            {
                templateUrl: 'partials/Registration.html',
                controller: 'appleIndexController'
            })
            .when('/login',
            {
                templateUrl: 'partials/login.html',
                controller: 'loginController'
            })
            .when('/ProfileType',
            {
                templateUrl: 'partials/userProfileType.html',
            })
            .when('/logout',
            {
                templateUrl: 'partials/login.html',
                controller: 'logoutController'
            })
            .when('/home',
            {
                templateUrl: 'partials/home.html'
                //controller: 'homeController'
            })
            .when('/changePassword',
            {
                templateUrl: 'partials/changePassword.html'
                //controller: 'changePasswordController'
            })
            .when('/contact',
            {
                templateUrl: 'partials/contact.html'
                //controller: 'homeController'
            })
            .when('/channelList',
            {
                templateUrl: 'partials/channelList.html',
                // controller: 'channelListController'
            })
            .when('/channelMessageCreate/:ChannelID',
            {
                templateUrl: 'partials/channelMessageCreate.html',
                //controller: 'channelMessageCreateController'
            })
            .when('/channelMessageList/:channelId',
            {
                templateUrl: 'partials/channelMessageList.html'
                // controller: 'channelMessageListController'
            })
            .when('/channelPastMessageList/:channelId',
            {
                templateUrl: 'partials/channelPastMessageList.html'
                // controller: 'channelMessageListController'
            })
            .when('/channelDetails/:channelId',
            {
                templateUrl: 'partials/channelDetails.html'
                //controller: 'channelDetailsController'
            })
           .when('/channelMessageView/:messageId',
           {
               templateUrl: 'partials/channelMessageView.html'
           })
           .when('/channelMessageEdit/:messageId',
           {
               templateUrl: 'partials/channelMessageEdit.html'
           })
           .when('/channelSubscriberList/:channelId',
           {
               templateUrl: 'partials/channelSubscriberList.html'
           })
           .when('/channelSubscriberDetails/:subscriberId',
           {
               templateUrl: 'partials/channelSubscriberDetails.html'
           })
           .when('/contact',
           {
               templateUrl: 'partials/contact.html'
           })
           .when('/subscriberChannelList/:subscriberId',
           {
               templateUrl: 'partials/subscriberChannelList.html'
           })
           .when('/newChannelsList/:subscriberId',
           {
               templateUrl: 'partials/newChannelsList.html'
           })
           .when('/channelSubscriberMessageList/:channelId',
           {
               templateUrl: 'partials/channelSubscriberMessageList.html'
           })
           .when('/channelFullDetails/:channelId',
           {
               templateUrl: 'partials/channelFullDetails.html'
           })
           .when('/subscriberMessageView/:messageId',
           {
               templateUrl: 'partials/subscriberMessageView.html'
           })
           .when('/channelSubscriberStatistics/:channelId',
           {
               templateUrl: 'partials/channelSubscriberStatistics.html'
           })
           .when('/channelSubscriberPastMessageList/:channelID',
           {
               templateUrl: 'partials/channelSubscriberPastMessageList.html'
           })
           .when('/ViewProfile',
           {
               templateUrl: 'partials/userProfile.html'
           })
           .when('/editProfile',
           {
               templateUrl: 'partials/userProfileEdit.html'
           })
           .when('/EditReview/:messageId',
           {
               templateUrl: 'partials/messageEditReview.html'
           })
           .when('/subscriberChannelMessage/:channelId',
           {
               templateUrl: 'partials/subscriberChannelMessage.html'
           })
            .when('/subscriberMessageView/:messageId',
           {
               templateUrl: 'partials/subscriberMessageView.html'
           })
              .when('/cart',
          {
              templateUrl: 'partials/shoppingCart.html'
          })
              .when('/subscriberChannelMessageview/:channelId',
          {
              templateUrl: 'partials/subscriberChannelMessageview.html'
          })
           .otherwise(
           {
               redirectTo: '/logout'
           });
      }]);


    //Initialize global variable
    prviiApp.run(function ($rootScope, $location) {
        $rootScope.SERVICE_URL = "http://localhost:53354/api/";                      // Local system WebApi link
        $rootScope.SERVICE_URL0 = "http://apiuat.prvii.org/api/";         // UAT Server WebApi link
        $rootScope.SERVICE_URL3 = "http://apistaging.prvii.org/api/";
        $rootScope.SERVICE_URL4 = "http://api.prvii.org/api/";    // WebApi link for live api.prvii.org 
        $rootScope.SERVICE_URL1 = "http://111.93.192.189/prviiapi/api/";  // WebApi link for my System 
        $rootScope.USER_ID = 0;
        $rootScope.USER_EMAIL = "";
        $rootScope.PASSWORD = "";
        $rootScope.USER_FIRST_NAME = "";
        $rootScope.USER_LAST_NAME = "";
        $rootScope.Full_NAME = "";
        $rootScope.USER_PROFILE_TYPE_ID = 0;
        $rootScope.USER_ChannelID = 0;
        $rootScope.USER_ChannelName = "";
        $rootScope.USER_GroupID = 0;
        $rootScope.USER_NickName = "";
        $rootScope.Authenticate = false;
        $rootScope.IsCelebrityUser = false;
        $rootScope.HomePath = '/home';
        $rootScope.IsSubscriber = false;

       
        //global/common utility function
        $rootScope.IsLoggedIn = function () {           
            return $rootScope.USER_ID != 0;
        };

        $rootScope.CheckSession = function () {
            if ($rootScope.USER_ID == 0)
                $location.path('/logout');
        };

        $rootScope.GetLoggedUser = function () {
            return { ID: $rootScope.USER_ID, Username: $rootScope.USER_EMAIL, Password: $rootScope.PASSWORD };
        };

        $rootScope.ShowMenu = function (menuType) {
            //debugger
            var showGroupMenu = false;
            var showCelebrityMenu = false;
            var showSubscriberMenu = false;

            if ($rootScope.USER_PROFILE_TYPE_ID == 1) //administrator
            {
                showGroupMenu = true;
                showCelebrityMenu = true;
                showSubscriberMenu = true;
            }
            else if($rootScope.USER_PROFILE_TYPE_ID == 2) //group
            {
                showGroupMenu = true;
               //showCelebrityMenu = true;
               // showSubscriberMenu = true;
            }
            else if ($rootScope.USER_PROFILE_TYPE_ID == 3) //celebrity
            {
                showCelebrityMenu = true;
               // showSubscriberMenu = true;
            }
            else if ($rootScope.USER_PROFILE_TYPE_ID == 4) //subscriber
            {
                showSubscriberMenu = true;
            }

            if (menuType == "group")
                return showGroupMenu;
            else if (menuType == "celebrity")
                return showCelebrityMenu;
            else if (menuType == "subscriber")
                return showSubscriberMenu;
        };
    });


    //prviiApp.directive('loading', function () {
    //    return {
    //        restrict: 'E',
    //        replace:true,
    //        template: '<div class="loading"><img src="http://www.nasa.gov/multimedia/videogallery/ajax-loader.gif" width="20" height="20" />LOADING...</div>',
    //        link: function (scope, element, attr) {
    //            scope.$watch('loading', function (val) {
    //                if (val)
    //                    $(element).show();
    //                else
    //                    $(element).hide();
    //            });
    //        }
    //    }
    //})

})();