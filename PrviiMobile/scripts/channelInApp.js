(function () {
    'use strict';
    var prviiAppchannelInApp = angular.module('prviiApp.channelInApp', []);

    prviiAppchannelInApp.controller('channelInAppController', ['$rootScope', '$scope', '$http', 'ngProgress',
function ($rootScope, $scope, $http, ngProgress) {
    $scope.show = false;
    ngProgress.start();
    var dataToPost = { ID: 0 };   
    $http.post($rootScope.SERVICE_URL + "channel/GetIosChannelList", dataToPost)
                .success(function (serverResponse, status, headers, config) {
                   //debugger
                    $scope.channelList = serverResponse;
                    onPageShow();

                }).error(function (serverResponse, status, headers, config) {
                    alert(status + " - Error Occured!");
                }
            );
    ngProgress.complete();
    $scope.show = true;


    $scope.GetIosCelebrity = function () {
        alert("getioscelebrity");
        $scope.show = false;
        ngProgress.start();
        var dataToPost = { ID: 0 };
        $http.post($rootScope.SERVICE_URL + "channel/GetIosChannelList", dataToPost)
                    .success(function (serverResponse, status, headers, config) {
                        //debugger
                        $scope.channelList = serverResponse;
                        onPageShow();

                    }).error(function (serverResponse, status, headers, config) {
                        alert(status + " - Error Occured!");
                    }
                );
        ngProgress.complete();
        $scope.show = true;
    }


    
    $scope.Loginpage = function () {
        $location.path('/login');
    }


    $scope.Buy =function (pid) {      
        debugger
        alert(device.platform);
        store.order(pid);
     
    }


    function onPageShow() {
        //debugger
        store.verbosity = store.DEBUG;
        //debugger
        angular.forEach($scope.channelList, function (value, key) {
            /* do something for all key: value pairs */           
            store.register({
                id: "io.cordova.PrviiMobile." + value.ID,
                alias: value.Name,
                type: store.CONSUMABLE
            });
        });
       

        //debugger

        store.when("product").updated(function (p) {
            render(p);
        });

        store.when("full version").approved(function (order) {
            order.finish();
        });

        store.refresh();
    }

    function render(product) {
        //debugger
        var elId = product.id;

       // debugger
        if (!product.loaded) {
            var model = $parse(elId);
            model.assign($scope, false);
            $scope.$apply();          
            return;
        }
        else if (!product.valid) {
            var model = $parse(elId);
            model.assign($scope, false);
            $scope.$apply();
        }
        else if (product.valid) {
           // debugger
            if (product.canPurchase) {
                var model = $parse(elId);
                model.assign($scope, true);
                $scope.$apply();
            }
            //if (product.canPurchase) {
            //    $(product.id).onclick = function (event) {
            //        var pid = this.getAttribute("id");
            //        store.order(pid);
            //    };
            //}
        }
    }
   
}]
)
})();