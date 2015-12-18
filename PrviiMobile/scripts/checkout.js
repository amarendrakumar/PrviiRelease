//var buyNowBtn = document.getElementById("buyNowBtn");

//buyNowBtn.onclick = function (e) {
//    alert('buy now!');
//    var paymentDetails = new PayPalPaymentDetails(
//      "15.00", // subtotal (amount ex shipping and tax)
//       "3.00", // shipping
//       "2.00"  // tax
//    );

//    var payment = new PayPalPayment(
//      "20.00", // amount (the sum of the fields above)
//      "USD",   // currency (in ISO 4217 format)
//      "Telerik T-Shirt", // description of the payment
//      "Sale",  // Sale (immediate payment) or Auth (authorization only)
//      paymentDetails // the object prepared above, optional
//    );

//    PayPalMobile.renderSinglePaymentUI(
//      payment,
//      function (payment) { alert("payment success: " + payment.response.id + "---" + JSON.stringify(payment)); },
//      function (errorresult) { alert(errorresult) }
//    );
//};


(function () {
    'use strict';

    //Declare App
    var prviiApp = angular.module('prviiApp.PayPal', []);
    prviiApp.controller('PayPalController', ['$scope', '$rootScope', '$http', '$location',
function ($scope, $rootScope, $http, $location) {

    $scope.Go = function Go() {
        alert("amar");
        alert('buy now!');
        var paymentDetails = new PayPalPaymentDetails(
          "15.00", // subtotal (amount ex shipping and tax)
           "3.00", // shipping
           "2.00"  // tax
        );

        var payment = new PayPalPayment(
          "20.00", // amount (the sum of the fields above)
          "USD",   // currency (in ISO 4217 format)
          "Telerik T-Shirt", // description of the payment
          "Sale",  // Sale (immediate payment) or Auth (authorization only)
          paymentDetails // the object prepared above, optional
        );

        PayPalMobile.renderSinglePaymentUI(
          payment,
          function (payment) { alert("payment success: " + payment.response.id + "---" + JSON.stringify(payment)); },
          function (errorresult) { alert(errorresult) }
        );
    }



}]);




})();