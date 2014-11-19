angular.module('shipApp').service('FirebaseDeliveryService', function ($firebase, FIREBASE_URL, AUTH_TOKEN) {
    var ref = new Firebase(FIREBASE_URL);
    ref.authWithCustomToken(AUTH_TOKEN, function (error) {
        if (error) {
            console.log("Login Failed!", error);
        } else {
            console.log("Login Succeeded!");
        }
    });
    var fbService =
      {
          CurrentDelivery:0,
          SaveScanned: function (item, delivery) {
              var items = $firebase(new Firebase(FIREBASE_URL + "Deliveries/"+delivery+"/scanned"));

              items.$push(item);
          },
          SaveNotScanned: function (item, delivery) {
              var items = $firebase(new Firebase(FIREBASE_URL + "Deliveries/" + delivery + "/notscanned"));
              items.$push(item);
          },

          GetDelivery: function (delivery) {
//              this.CurrentDelivery = delivery;
              var items = $firebase(new Firebase(FIREBASE_URL + "Deliveries/"+delivery)).$asObject();
              return items;
          },
          Delete: function(delivery) {
              return $firebase(new Firebase(FIREBASE_URL + "Deliveries/" + delivery.DeliveryNumber)).$remove();
          }
      };
    return fbService;
});