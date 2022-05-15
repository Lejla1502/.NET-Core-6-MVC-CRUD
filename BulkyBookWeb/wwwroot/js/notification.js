////"use strict";

<<<<<<< HEAD
////var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();

//////Disable the send button until connection is established.
////document.getElementById("sendButton").disabled = true;

////connection.on("ReceiveMessage", function (user, message) {
////    var li = document.createElement("li");
////    document.getElementById("messagesList").appendChild(li);
////    // We can assign user-supplied strings to an element's textContent because it
////    // is not interpreted as markup. If you're assigning in any other way, you 
////    // should be aware of possible script injection concerns.
////    li.textContent = `${user} says ${message}`;
////});

////connection.start().then(function () {
////    document.getElementById("sendButton").disabled = false;
////}).catch(function (err) {
////    return console.error(err.toString());
////});

////document.getElementById("sendButton").addEventListener("click", function (event) {
////    var user = document.getElementById("userInput").value;
////    var message = document.getElementById("messageInput").value;
////    connection.invoke("SendMessage", user, message).catch(function (err) {
////        return console.error(err.toString());
////    });
////    event.preventDefault();
////});
=======
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function ( message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${message}`;
});

connection.start().then(function () {
   // document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


$(document).ready( function () {
    //var user = document.getElementById("userInput").value;
    //var message = document.getElementById("messageInput").value;
    var message = "User has purchased an order";
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});
>>>>>>> 5c761ea85d7c842ac157a16facd276828fef0aca
