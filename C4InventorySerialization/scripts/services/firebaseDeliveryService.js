angular.module('shipApp').service('FirebaseDeliveryService', function ($firebase, FIREBASE_URL) {

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
          }
      };
    return fbService;
});