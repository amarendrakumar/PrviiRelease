(function () {
    'use strict';
    var prviiAppHome = angular.module('prviiApp.home', []);

    prviiAppHome.controller('HomeController', ['$scope', '$rootScope', '$http', '$location', 'ngProgress',
function ($scope, $rootScope, $http, $location, ngProgress) {
    $scope.show = false;
    ngProgress.start();
    //$scope.templates=[];

    //$scope.include = function(templateURI) {
    //    var template={url:templateURI};
    //    $scope.templates.push(template);
    //}

    //$scope.delete= function(url){
    //    removeEntity($scope.templates,url);
    //}

    //var removeEntity = function(elements,url){
    //    elements.forEach(function(element,index){
    //        if(element.url===url){
    //            elements.splice(index,1);
    //            removeEntity(elements,url);
    //        }
    //    })
    $rootScope.group = false;
    $rootScope.celeberity = false;
    $rootScope.subscriber = false;
    $rootScope.contact = false;
    $rootScope.subscribchannelList = false;

    $rootScope.templates = [{ src: 'template' }];

    $rootScope.include = function (templateURI) {
        $rootScope.templates.push({ src: 'partials/' + templateURI });
    }

    $rootScope.deleteSource = function (index) {
        $rootScope.templates.splice(index, 1);
    }


    $rootScope.includeGroup = function (templateURI, isTrue) {
        $rootScope.srcGroup = 'partials/' + templateURI;
       
        if (isTrue)
            $rootScope.group = false;
        else
            $rootScope.group = true;

        $rootScope.celeberity = false;
        $rootScope.subscriber = false;
        $rootScope.contact = false;
    }
    $rootScope.includeCelebrity = function (templateURI, isTrue) {
        $rootScope.srcCelebrity = 'partials/' + templateURI;
        $rootScope.group = false;
        if (isTrue)
            $rootScope.celeberity = false;
        else
            $rootScope.celeberity = true;

        $rootScope.subscriber = false;
        $rootScope.contact = false;
    }

    $rootScope.includeSubscriber = function (templateURI, isTrue) {
        ngProgress.start();
        $scope.show = false;
        $rootScope.srcSubscriber = 'partials/' + templateURI;
        $rootScope.group = false;
        $rootScope.celeberity = false;
   
        if (isTrue)
        {
            $rootScope.subscribchannel = false;          
        }           
        else
        {
            $rootScope.subscribchannel = true;           
        }
            

        $rootScope.contact = false;

        ngProgress.complete();
        $scope.show = true;
    }

    $rootScope.subscribchannel = false;
    $rootScope.channel = false;
    $rootScope.includeSubscriberChannel = function (templateURI, isTrue) {
             
        ngProgress.start();
        $scope.show = false;
           
        $rootScope.srcSubscriber = 'partials/' + templateURI;
        $rootScope.channel = false;
        $rootScope.srcChannel = "";
        
        if (isTrue) {          
            $rootScope.subscribchannelList = false;
        }
        else {           
            $rootScope.subscribchannelList = true;
        }

        ngProgress.complete();
        $rootScope.show = true;
    }


    $rootScope.includeChannel = function (templateURI, isTrue) {
        ngProgress.start();
        $scope.show = false;        
        $rootScope.srcChannel = 'partials/' + templateURI;
        $rootScope.subscribchannelList = false;
        $rootScope.srcSubscriber = "";

        if (isTrue) {
            $rootScope.channel = false;
        }
        else {
            $rootScope.channel = true;
        }

        ngProgress.complete();
        $scope.show = true;
    }

    $rootScope.includeSubscriberChannelFullDetails = function (templateURI,channelid) {
       
        $rootScope.srcChannel = 'partials/' + templateURI;
        $rootScope.channel = true;
        $rootScope.channelId = channelid;
    }

    $rootScope.includeContact = function (templateURI, isTrue) {
        $rootScope.srcContact = 'partials/' + templateURI;
        $rootScope.group = false;
        $rootScope.celeberity = false;
        $rootScope.subscriber = false;
       
        if (isTrue)
            $rootScope.contact = false;
        else
            $rootScope.contact = true;
    }


    $rootScope.includeSubscriberchannelMessage = function (templateURI, channelID) {
        //alert(channelID + templateURI);
        $rootScope.srcSubscriber = "";
        $rootScope.srcSubscriber = 'partials/' + templateURI;
        $rootScope.subscribchannelList = true;
        $rootScope.channelId = channelID;
    }



    


}]);

})();