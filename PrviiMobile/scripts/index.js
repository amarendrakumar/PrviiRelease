// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in Ripple or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.
(function () {
    "use strict";

    document.addEventListener( 'deviceready', onDeviceReady.bind( this ), false );
    //document.addEventListener('push-notification', function (event) {
    //    debugger
    //    var title = event.notification.title;
    //    var userData = event.notification.userdata;

    //    if (typeof (userData) != "undefined") {
    //        console.warn('user data: ' + JSON.stringify(userData));
    //    }
    //   // alert(title);
    //});

    function onDeviceReady() {
        // Handle the Cordova pause and resume events
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);
        var deviceregistrationid = localStorage.getItem('deviceregistrationid');
        //set push notifications handler
       
       // alert("Amar");
       // debugger
      // alert(device.platform);
        // TODO: Cordova has been loaded. Perform any initialization that requires Cordova here.
        // debugger
        document.addEventListener("online", onOnline.bind(this), false);
        document.addEventListener("offline", onOffline.bind(this), false);

       //alert(device.platform);
        document.addEventListener('backbutton', function (event) {
            event.preventDefault(); // EDIT
            navigator.app.exitApp(); // exit the app
        });

        // TODO: Cordova has been loaded. Perform any initialization that requires Cordova here.
        //  initPaymentUI(); //initialize paypal ui


        console.log("window.open works well");
        // checkConnection();

        var push = PushNotification.init({
            "android": {
                "senderID": "744011430246"
            },
            "ios": { "alert": "true", "badge": "true", "sound": "true" },
            "windows": {}
        });

        push.on('registration', function (data) {            
            localStorage.setItem('deviceregistrationid', data.registrationId);
            console.log("registration event");
            document.getElementById("regId").innerHTML = data.registrationId;
            console.log(JSON.stringify(data));
        });

        push.on('notification', function (data) {
            console.log("notification event");
            console.log(JSON.stringify(data));
            var cards = document.getElementById("cards");
            var card = '<div class="row">' +
		  		  '<div class="col s12 m6">' +
				  '  <div class="card darken-1">' +
				  '    <div class="card-content black-text">' +
				  '      <span class="card-title black-text">' + data.title + '</span>' +
				  '      <p>' + data.message + '</p>' +
				  '    </div>' +
				  '  </div>' +
				  ' </div>' +
				  '</div>';
            cards.innerHTML += card;

            push.finish(function () {
                console.log('finish successfully called');
            });
        });

        push.on('error', function (e) {
            console.log("push error");
        });
    };

    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };

    function onOnline() {
        // Handle the online event
        // alert('Connection onOnline event: ');
        checkConnection();
    }


    function onOffline() {
        // Handle the offline event
        // alert('Connection onOffline event: ');
        checkConnection();
        alert("No Internet Connection, Please connect to Internet and Continue");
        //window.location.href = "index.html";
      
    }


  
    function checkConnection() {
        var networkState = navigator.connection.type;

        var states = {};
        states[Connection.UNKNOWN] = 'Unknown connection';
        states[Connection.ETHERNET] = 'Ethernet connection';
        states[Connection.WIFI] = 'WiFi connection';
        states[Connection.CELL_2G] = 'Cell 2G connection';
        states[Connection.CELL_3G] = 'Cell 3G connection';
        states[Connection.CELL_4G] = 'Cell 4G connection';
        states[Connection.CELL] = 'Cell generic connection';
        states[Connection.NONE] = 'No network connection';

        // alert('Connection type: ' + states[networkState]);
    }


    function initPaymentUI() {
        var clientIDs = {
            "PayPalEnvironmentProduction": "YOUR_PRODUCTION_CLIENT_ID",
            "PayPalEnvironmentSandbox": "AZAm1RAlrdDzXRBC5h8KPThw9BcdHzjtGs4lp1SQgl73PaR0wpVqveJimO0v"
        };
        PayPalMobile.init(clientIDs, onPayPalMobileInit);
    };

    function onPayPalMobileInit() {
        // must be called
        // use PayPalEnvironmentNoNetwork mode to get look and feel of the flow
        PayPalMobile.prepareToRender("PayPalEnvironmentSandbox", configuration(), onPrepareRender);
    };

    function configuration() {
        // for more options see `paypal-mobile-js-helper.js`
        var config = new PayPalConfiguration({
            merchantName: "My test shop",
            merchantPrivacyPolicyURL: "https://mytestshop.com/policy",
            merchantUserAgreementURL: "https://mytestshop.com/agreement"
        });
        return config;
    };

    function onPrepareRender() {
        console.log("OK, ready to accept payments!");
    };

   


} )();