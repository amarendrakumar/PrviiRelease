//(function (angular) {
//    'use strict';
//    angular.module('ngSwipeLeftExample', ['ngTouch']);
//})(window.angular);


var app = angular.module('prviiApp.progressApp', ['ngProgress']);

app.config(function (ngProgressProvider) {
    // Default color is firebrick
   // ngProgressProvider.setColor('firebrick');
    ngProgressProvider.setColor('Chartreuse');
    
    // Default height is 2px
    ngProgressProvider.setHeight('10px');
});

var MainCtrl = function ($scope, $timeout, ngProgress) {
  //  $scope.name = 'Lars';
   // $scope.show = false;
   // debugger
    $scope.color = ngProgress.color();
    $scope.height = ngProgress.height();

    ngProgress.start();
    $timeout(function () {
        ngProgress.complete();
        $scope.show = true;
    }, 2000);

    $scope.setWidth = function (new_width, $event) {
       // debugger
       // alert("djd");
        ngProgress.set(new_width);
        $event.preventDefault();
    }

    $scope.startProgress = function ($event) {
        $event.preventDefault();
        ngProgress.start();
    }

    $scope.count = function ($event) {
        $event.preventDefault();
        ngProgress.set(ngProgress.status() + 9);
    }

    $scope.new_color = function (color, $event) {
        $event.preventDefault();
        ngProgress.color(color);
    }

    $scope.new_height = function (new_height, $event) {
        $event.preventDefault();
        ngProgress.height(new_height);
    }

    $scope.completeProgress = function ($event) {
        $event.preventDefault();
        ngProgress.complete();
    }

    $scope.stopProgress = function ($event) {
        $event.preventDefault();
        ngProgress.stop();
    }

    $scope.resetProgress = function ($event) {
        ngProgress.reset();
        $event.preventDefault();
    }
}